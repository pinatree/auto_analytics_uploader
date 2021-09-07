using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using OfficeOpenXml;

using AutoAnalytics.Upload.Model.DataTypes.XlsxV1;
using AutoAnalytics.Upload.Uploader;
using AutoAnalytics.Upload.PostgresConnector;

namespace dreamscape_upload_lib
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteHello();
            InitLibs();

            string path = @"D:\AutoDefectsCatalog.xlsx";

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            PgUploader kek = new PgUploader();

            List<BreakdownInfo> srcBreakDowns = kek.Read(path);

            Console.WriteLine("--------- groups ---------");


            List<TGroup> srcGroups = kek.GetGroups(srcBreakDowns);
            List<TGroup> filledGroups = kek.UploadGroups(srcGroups);

            foreach (var item in filledGroups)
            {
                Console.WriteLine(item.Id + " " + item.CName);
            }


            Console.WriteLine("--------- subgroups ---------");


            List<TSubgroup> srcSubgroups = kek.GetSubgroups(filledGroups, srcBreakDowns).OrderBy(x => x.GroupId).ToList();
            List<TSubgroup> filledSubgroups = kek.UploadSubgroups(srcSubgroups).OrderBy(x => x.GroupId).ThenBy(x => x.CName).ToList();

            foreach (var item in filledSubgroups)
            {
                Console.WriteLine(item.GroupId + "  " + item.CName);
            }


            Console.WriteLine("--------- details ---------");

            List<TDetail> srcDetails = kek.GetDetails(filledSubgroups, srcBreakDowns).OrderBy(x => x.SubgroupId).ToList();
            List<TDetail> filledDetails = kek.UploadDetails(srcDetails).OrderBy(x => x.SubgroupId).ThenBy(x => x.CName).ToList();

            foreach (var item in filledDetails)
            {
                Console.WriteLine(item.SubgroupId + "   " + item.CName);
            }

            Console.WriteLine("--------- crashes ---------");

            List<TCrash> srcCrashes = kek.GetCrashes(filledDetails, srcBreakDowns).OrderBy(x => x.DamageDetailId).ThenBy(x => x.DamageDetailId).ToList();

            foreach (var item in srcCrashes)
            {
                Console.WriteLine(item.Id + "   " + item.CrashId);
            }

            //List<TCrash> filledCrashes = kek.GetCrashes(srcCrashes);

            //List<BreakdownInfo> readed = new PgUploader().Read(path);//ExcelReader.Read(path).ToList();
            //var groupNames = new PgUploader().GetGroups(readed);

            //foreach (var item in groupNames)
            //{
            //    Console.WriteLine(item.CName + " ");
            //}


            //foreach (var item in RecordsTransformator.GetSubgroupNames(readed))
            //{
            //    Console.WriteLine(item.GroupName + "    " + item.Name);
            //}
            //foreach (var item in RecordsTransformator.GetDetailNames(readed))
            //{
            //    Console.WriteLine(item.GroupName + "    " + item.SubgroupName + "  " + item.Name);
            //}

            //Конвертируем группы, подгруппы, и детали в формат базы данных (с id'шниками)
            //Передаем их в Uploader, назоваем там методы что-то вроде Fill_id
            //Сделали с группами - проходим по подгруппам, и там заполняем ID'шники групп
            //Далее с деталями.
            //Когда все получилось, у нас есть отражение всех 3 сущностей как в БД.
            //Загружаем случаи
            //Запускаем перегенерацию правил
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
}
