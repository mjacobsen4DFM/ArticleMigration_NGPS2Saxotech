1) ArticleDataServiceMigration
	loads tables: article, asset, image

arguments: siteid startdate enddate logfile

siteid is the ngps siteid ( Our current CMS)

2) ImageMigration 
loads table: saxo_image

arguments: siteid  destination_siteid startdate enddate logfile

where destination_siteid is saxotech target publication


3) GalleryMigration
loads table: saxo_gallery

arguments: siteid  destination_siteid startdate enddate logfile



Steps 2 & 3 use the saxo_categoryMap to map old categories to saxotech new categories and a taxononomy key.
They both post to saxotech system through OWC ..
Step 3 can not be run before step 2. It uses the saxotech url returned from step 2 to embedd in saxotech xml to create a gallery,

4) CreateSaxoArticle

creates saxotech specific xml using 
tables: article , image, saxo_image, saxo_gallery

updates:  saxo_article

5) PostArticles 

At this point we have all the information to post 
the articles from table: saxo_article field: xmldata 


6) Optional SmugImageMigration

Using tables: article, image, saxo_gallery push galleries to smugmug...







