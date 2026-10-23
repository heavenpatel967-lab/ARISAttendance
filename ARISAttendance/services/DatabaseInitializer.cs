using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Dapper;

namespace ARISAttendance.services
{
    public static class DatabaseInitializer
    {
        private const string DbFileName = "ARISAttendanceDB.sqlite";
        public static string ConnectionString = $"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbFileName)}";

        public static void Initialize()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                try
                {
                    connection.Execute("SELECT Punch5 FROM DailyAttendance LIMIT 1");
                }
                catch
                {
                    connection.Execute("DROP TABLE IF EXISTS DailyAttendance;");
                }

                string createTablesQuery = @"
                    CREATE TABLE IF NOT EXISTS Employees (
                        EmpCode VARCHAR(20) PRIMARY KEY,
                        CardNo VARCHAR(20),
                        FullName VARCHAR(100),
                        Department VARCHAR(50),
                        Category VARCHAR(50),
                        Shift VARCHAR(10)
                    );

                    CREATE TABLE IF NOT EXISTS RawPunches (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        EmpCode VARCHAR(20),
                        PunchTime DATETIME
                    );

                    CREATE TABLE IF NOT EXISTS DailyAttendance (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        EmpCode VARCHAR(20),
                        WorkDate DATE,
                        PunchIn DATETIME NULL,
                        LunchOut DATETIME NULL,
                        LunchIn DATETIME NULL,
                        PunchOut DATETIME NULL,
                        Punch5 DATETIME NULL,
                        Punch6 DATETIME NULL,
                        Punch7 DATETIME NULL,
                        Punch8 DATETIME NULL,
                        IsException BIT
                    );

                    CREATE TABLE IF NOT EXISTS SpecialRules (
                        RuleType VARCHAR(50),
                        EmpCode VARCHAR(20),
                        UNIQUE(RuleType, EmpCode)
                    );
                    
                    -- NEW TABLE: Remembers app settings like the last export folder path
                    CREATE TABLE IF NOT EXISTS AppSettings (
                        SettingKey VARCHAR(50) PRIMARY KEY,
                        SettingValue TEXT
                    );
                    
                    INSERT OR IGNORE INTO SpecialRules (RuleType, EmpCode) VALUES ('AutoFillAll', '0555');
                    INSERT OR IGNORE INTO SpecialRules (RuleType, EmpCode) VALUES ('ShiftEarlyLunch', '1622');
                    INSERT OR IGNORE INTO SpecialRules (RuleType, EmpCode) VALUES ('ShiftEarlyLunch', '1963');
                    
                    INSERT OR IGNORE INTO SpecialRules (RuleType, EmpCode) VALUES ('AutoFillAll', '0003');
                    INSERT OR IGNORE INTO SpecialRules (RuleType, EmpCode) VALUES ('AutoFillAll', '0004');
                    INSERT OR IGNORE INTO SpecialRules (RuleType, EmpCode) VALUES ('AutoFillAll', '0005');
                ";

                using (var command = new SqliteCommand(createTablesQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}