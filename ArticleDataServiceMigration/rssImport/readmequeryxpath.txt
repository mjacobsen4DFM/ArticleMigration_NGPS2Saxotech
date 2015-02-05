




SELECT item.value('(/item/extended/category-id)[1]',
  'varchar(max)') AS category
FROM minedRSS 

SELECT item.value('(/item/extended/category-id)[1]',
  'varchar(max)') AS category
FROM minedRSS where item.exist('/item/extended/category-id[. =  "1030349"]') = 1

select xmlprofiles.query('//profiles') from minedRSS where xmlprofiles.exist('//profiles/profile[. = "3020825"]') = 1