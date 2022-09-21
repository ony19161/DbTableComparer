--select name, id from sysobjects where xtype='U' and name in ('SourceTable1', 'SourceTable2')


-- query for added rows in new table
Select * from SourceTable2 where SocialSecurityNumber not in (Select SocialSecurityNumber from SourceTable1
)

-- query for removed rows in old table
Select * from SourceTable1 where SocialSecurityNumber not in (
 
Select SocialSecurityNumber from SourceTable2
)

-- query for getting changed rows
Select * from (((SELECT *,'SourceTable1' as TableName FROM SourceTable1 except SELECT *,'SourceTable2'  as TableName FROM SourceTable2) 
  union (SELECT *,'SourceTable2' as TableName FROM SourceTable2 except SELECT *,'SourceTable1' as TableName FROM SourceTable1))
except 
(Select *,'SourceTable2' as TableName from SourceTable2 where SocialSecurityNumber not in (Select SocialSecurityNumber from SourceTable1) 
 union (Select *,'SourceTable1' as TableName from SourceTable1 where SocialSecurityNumber not in (Select SocialSecurityNumber from SourceTable2)))) as T
 order by T.SocialSecurityNumber

