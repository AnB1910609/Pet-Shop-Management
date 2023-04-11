Create Database PetShopManagement
Use PetShopManagement

Create Table CustomerTbl(
	CustId int Primary Key IDENTITY not null,
	CustName nvarchar(50) Not Null,
	CustPhone nvarchar(10) unique not null,
	CustAdd nvarchar(100) Not Null
)

Create Table EmployeeTbl(
	EmpNum int Primary Key IDENTITY not null,
	EmpName nvarchar(50) Not Null,
	EmpPhone nvarchar(10) not null unique,
	EmpDOB date not null,
	EmpAdd nvarchar(50) Not Null
)

Create Table CategoryTbl(
	CatId int Primary Key IDENTITY not null,
	CatName nvarchar(50) Not Null
)

Create Table ProductTbl(
	PrId int Primary Key IDENTITY not null,
	PrName nvarchar(50) Not Null,
	CatId int Not Null,
	PrQty int not null,
	PrPrice int not null,
	FOREIGN KEY (CatId)
	REFERENCES CategoryTbl (CatId)
)

Create Table BillTbl(
	BNum int Primary Key IDENTITY not null,
	CustId int Not Null,
	PrId int Not Null,
	EmpNum int Not Null,
	Qty int not null,
	Price int not null,
	BDateOP date Not Null,
	Total int not null,
	FOREIGN KEY (CustId)
	REFERENCES CustomerTbl (CustId), 
	FOREIGN KEY (PrId)
	REFERENCES ProductTbl (PrId), 
	FOREIGN KEY (EmpNum)
	REFERENCES EmployeeTbl (EmpNum)
)

SELECT ProductTbl.PrId, ProductTbl.PrName, CategoryTbl.CatName, ProductTbl.PrQty, ProductTbl.PrPrice
FROM ProductTbl, CategoryTbl
WHERE ProductTbl.PrCat=CategoryTbl.CatId

SELECT BillTbl.BNum, CustomerTbl.CustName, ProductTbl.PrName, EmployeeTbl.EmpName, BillTbl.Qty, BillTbl.Price, BillTbl.BDateOP, BillTbl.Total
FROM BillTbl, CustomerTbl, ProductTbl, EmployeeTbl
WHERE BillTbl.CustId=CustomerTbl.CustId and BillTbl.PrId = ProductTbl.PrId and BillTbl.EmpNum = EmployeeTbl.EmpNum