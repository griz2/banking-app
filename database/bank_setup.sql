-- one-time use of this script to create the database and tables for the banking project

create database bankingProjectDB;
GO
use bankingProjectDB;

create table Admins
(
    AdminId int identity(1,1),
    Username varchar(50) not null,
    Password varchar(200) not null,

    constraint pk_AdminId primary key(AdminId),
    constraint uk_AdminUsername unique(Username)
)

create table Accounts
(
    AccNo int,
    AccName varchar(100) not null,
    AccType varchar(20) not null,
    AccBalance float not null,
    Username varchar(50) not null,
    Password varchar(200) not null,

    constraint pk_AccNo primary key(AccNo),
    constraint uk_AccountUsername unique(Username),
    constraint chk_AccNo check(AccNo > 0),
    constraint chk_AccType check(AccType in ('Savings', 'Checking')),
    constraint chk_AccBalance check(AccBalance >= 0)
)

create table BankTransactions
(
    TransactionId int identity(1,1),
    AccNo int not null,
    TransactionType varchar(20) not null,
    Amount float not null,
    BalanceAfter float not null,
    RelatedAccNo int,
    TransactionDate datetime not null,

    constraint pk_TransactionId primary key(TransactionId),
    constraint fk_TransactionAccNo foreign key(AccNo) references Accounts(AccNo),
    constraint fk_RelatedAccNo foreign key(RelatedAccNo) references Accounts(AccNo),
    constraint chk_TransactionType check(TransactionType in ('Opening Deposit', 'Deposit', 'Withdraw', 'Transfer In', 'Transfer Out')),
    constraint chk_TransactionAmount check(Amount > 0),
    constraint chk_BalanceAfter check(BalanceAfter >= 0)
)

create table ChequeRequests
(
    RequestId int identity(1,1),
    AccNo int not null,
    Status varchar(20) not null,
    RequestDate datetime not null,
    ApprovalDate datetime,

    constraint pk_RequestId primary key(RequestId),
    constraint fk_RequestAccNo foreign key(AccNo) references Accounts(AccNo),
    constraint chk_RequestStatus check(Status in ('Pending', 'Approved'))
)

insert into Admins values('admin', 'admin123')

insert into Accounts values(101, 'Michael', 'Savings', 1000, 'customer', 'customer123')
insert into BankTransactions(AccNo, TransactionType, Amount, BalanceAfter, TransactionDate)
values(101, 'Opening Deposit', 1000, 1000, getdate())

insert into Accounts values(102, 'John', 'Checking', 500, 'customer2', 'customer456')
insert into BankTransactions(AccNo, TransactionType, Amount, BalanceAfter, TransactionDate)
values(102, 'Opening Deposit', 500, 500, getdate())
