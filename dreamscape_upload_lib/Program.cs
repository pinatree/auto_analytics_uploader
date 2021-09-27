using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using OfficeOpenXml;

using AutoAnalytics.Upload.Model.DataTypes.XlsxV1;
using AutoAnalytics.Upload.Uploader;
using AutoAnalytics.Upload.PostgresConnector;
using System.Configuration;
using Accord.MachineLearning.Rules;

namespace dreamscape_upload_lib
{
    class Program
    {
        static void Main(string[] args)
        {
            //Read connection string from config file
            string ip = ConfigurationManager.AppSettings["ip"];
            string dbName = ConfigurationManager.AppSettings["dbName"];
            string username = ConfigurationManager.AppSettings["username"];
            string dbPasswd = ConfigurationManager.AppSettings["dbPasswd"];

            string connStr = "Host=" + ip + ";Database=" + dbName + ";Username=" + username + ";Password=" + dbPasswd;

            //Test connection with this connection string
            //return;
            //
            WriteHello();
            InitLibs();

            //string path = @"D:\AutoDefectsCatalog.xlsx";

            //FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            //PgUploader kek = new PgUploader(connStr);

            //List<BreakdownInfo> srcBreakDowns = kek.Read(path);

            //Console.WriteLine("--------- groups ---------");


            //List<TGroup> srcGroups = kek.GetGroups(srcBreakDowns);
            //List<TGroup> filledGroups = kek.UploadGroups(srcGroups);

            //foreach (var item in filledGroups)
            //{
            //    Console.WriteLine(item.Id + " " + item.CName);
            //}


            //Console.WriteLine("--------- subgroups ---------");


            //List<TSubgroup> srcSubgroups = kek.GetSubgroups(filledGroups, srcBreakDowns).OrderBy(x => x.GroupId).ToList();
            //List<TSubgroup> filledSubgroups = kek.UploadSubgroups(srcSubgroups).OrderBy(x => x.GroupId).ThenBy(x => x.CName).ToList();

            //foreach (var item in filledSubgroups)
            //{
            //    Console.WriteLine(item.GroupId + "  " + item.CName);
            //}


            //Console.WriteLine("--------- details ---------");

            //List<TDetail> srcDetails = kek.GetDetails(filledSubgroups, srcBreakDowns).OrderBy(x => x.SubgroupId).ToList();
            //List<TDetail> filledDetails = kek.UploadDetails(srcDetails).OrderBy(x => x.SubgroupId).ThenBy(x => x.CName).ToList();

            //foreach (var item in filledDetails)
            //{
            //    Console.WriteLine(item.SubgroupId + "   " + item.CName);
            //}

            //Console.WriteLine("--------- crashes ---------");

            //List<TCrash> srcCrashes = kek.GetCrashes(filledDetails, srcBreakDowns).OrderBy(x => x.DamageDetailId).ThenBy(x => x.DamageDetailId).ToList();

            //foreach (var item in srcCrashes)
            //{
            //    Console.WriteLine(item.Id + "   " + item.CrashId);
            //}

            //Считываем всю инфу об авариях
            Console.WriteLine("Считывание информации об рекламациях...");

            dreamscape_portal_dbContext context = new dreamscape_portal_dbContext(connStr);
            var crashes = context.TCrashes.Select(x => x).ToList();

            foreach (var item in crashes)
                Console.WriteLine(item.Id);

            Console.WriteLine("Итого считано " + crashes.Count + " записей");

            Console.WriteLine("Объединение записей по рекламациям...");

            //Группируем их по инцидентам (1 авария)
            CrashInfo[] crashInfos = crashes.Select(c => c.CrashId).
                Distinct().
                Select(c => new CrashInfo()
                {
                    CrashKey = c
                }).ToArray();

            foreach (var item in crashInfos)
            {
                item.DetailIds = crashes.Where(x => x.CrashId == item.CrashKey).Select(x => Convert.ToInt32(x.DamageDetailId)).ToArray();
            }

            Console.WriteLine("Итого " + crashInfos.Count() + " рекламаций");

            Console.WriteLine("Удаление рекламаций с единственной записью как непригодных для анализа ассоциативных правил...");

            //Убираем те, где только 1 участник

            crashInfos = crashInfos.Where(x => x.DetailIds.Count() > 1).ToArray();
            Console.WriteLine(crashInfos.Count());

            Console.WriteLine("Итого " + crashInfos.Count() + " пригодных для анализа рекламаций");

            //Разбиваем это все на id-шники деталей внутри одной аварии

            List<SortedSet<int>> creashesForAssocAnalysis = new List<SortedSet<int>>();

            foreach(var item in crashInfos)
            {
                SortedSet<int> crashDataSet = new SortedSet<int>();

                foreach (var detail in item.DetailIds)
                    crashDataSet.Add(detail);

                creashesForAssocAnalysis.Add(crashDataSet);
            }

            //генерируем ассоциативные правила

            Apriori<int> apriori = new Apriori<int>(2, 0.1f);

            AssociationRuleMatcher<int> generated = apriori.Learn(creashesForAssocAnalysis.ToArray());

            IEnumerable<TAssocRule> assocRules = generated.Rules.Select(x => new TAssocRule()
            {
                CCalcDate = DateTime.Now,
                ConseqDetailId = x.Y.First(),
                ReasonDetailId = x.X.First(),
                CSupportCount = Convert.ToInt64(x.Support),
                CReliability = Convert.ToInt64(x.Confidence),
            });

            foreach (var item in context.TAssocRules)
                context.TAssocRules.Remove(item);

            context.SaveChanges();

            foreach (var item in assocRules)
            {
                context.TAssocRules.Add(item);
                context.SaveChanges();
            }                

            //Конвертируем группы, подгруппы, и детали в формат базы данных (с id'шниками)
            //Передаем их в Uploader, назоваем там методы что-то вроде Fill_id
            //Сделали с группами - проходим по подгруппам, и там заполняем ID'шники групп
            //Далее с деталями.
            //Когда все получилось, у нас есть отражение всех 3 сущностей как в БД.
            //Загружаем случаи
        }

        /*
 * Делаем DISTINCT
 * Чекаем, что уже есть в БД
 * Если есть - считываем ключ
 * Если нет - добавляем и считываем ключ
 * ...
 * ...
 * Далее с этими ключами работаем, добавляя новые узлы
 * 
 */

        static void WriteHello()
        {
            Console.WriteLine("Welcome to auto analytics uploader!");
            Console.WriteLine("This program allows you to load data " +
                "into the postgresql database from an.excel file, " +
                "and start processing association rules using stored " +
                "procedures");
        }

        static void InitLibs()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }
    }

    public class CrashInfo
    {
        public string CrashKey { get; set; }

        public int[] DetailIds { get; set; }
    }
}
