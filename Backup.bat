@echo off

for /f "tokens=1-4 delims=/ " %%i in ("%date%") do (

     set dow=%%i

     set month=%%j

     set day=%%k

     set year=%%l
   
)

set datestr=%month%_%day%_%year%

set BACKUP_FILE=irwan_%datestr%.sql

set PGPASSWORD=admin123

"C:\Program Files\PostgreSQL\9.5\bin\pg_dump.exe" -h localhost -d karental -U postgres -p 5432 --column-inserts -f D:\BackupDB\%BACKUP_FILE%