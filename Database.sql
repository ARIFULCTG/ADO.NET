drop database if exists DBstring
go
create database DBstring
go
use DBstring
go
create table Donor
(
ID varchar(100) primary key,
Name varchar(100),
BloodGroup varchar(100),
Number varchar(100),
DonationDate datetime,
Photo image,
stringphoto varchar(100)
)
go 
create  table Patient(
ID int primary key,
Name varchar(100),
Number varchar(100),
DonorID varchar(100) references Donor(ID)
)

select * from Donor;
select * from Patient;