select name, id from sysobjects where xtype='U' and name in ('SourceTable1', 'SourceTable2')


-- query for added rows in new table
Select * from SourceTable2 where SocialSecurityNumber not in (
 
Select SocialSecurityNumber from SourceTable1
)

-- query for removed rows in old table
Select * from SourceTable1 where SocialSecurityNumber not in (
 
Select SocialSecurityNumber from SourceTable2
)

-- query for getting changed rows
((SELECT * FROM SourceTable1 except SELECT * FROM SourceTable2) 
  union (SELECT * FROM SourceTable2 except SELECT * FROM SourceTable1))
except 
(Select * from SourceTable2 where SocialSecurityNumber not in (Select SocialSecurityNumber from SourceTable1) 
 union (Select * from SourceTable1 where SocialSecurityNumber not in (Select SocialSecurityNumber from SourceTable2)))


