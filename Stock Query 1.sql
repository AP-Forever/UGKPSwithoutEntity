USE UGKPS


SELECT * 
FROM UGKPS.dbo.Stocks_Investment
--84 rows


SELECT *
FROM UGKPS.dbo.Stocks_Transactions
WHERE Stock_Transaction_ID > 15
ORDER BY Stock_Transaction_ID
--93 rows inserted



--INSERT INTO Stocks_Transactions
SELECT UserID = 1, Ticker = Symbol, Exchange = 'TSX', CompanyName = Symbol, Price = CAST(Price AS FLOAT), Quantity = CAST(Quantity AS FLOAT), CreatedDate = [Transaction Date], LastUpdated = [Transaction Date]
FROM UGKPS.dbo.Questrade_Activities
--(93 rows affected)


SELECT Stock_Transaction_ID, UserID, Ticker, Exchange, CompanyName, Price, Quantity, Value = Price * Quantity, CreatedDate, LastUpdated
FROM UGKPS.dbo.Stocks_Transactions
WHERE Stock_Transaction_ID = 34
ORDER BY Ticker, CreatedDate, Quantity DESC
--93 rows inserted


UPDATE t SET
--SELECT *,
	Ticker = REPLACE(REPLACE(REPLACE(Ticker, '.TO', ''), '.CN', ''), '.VN', '')
FROM UGKPS.dbo.Stocks_Transactions t
WHERE Ticker LIKE '%.%'
--(84 rows affected)


