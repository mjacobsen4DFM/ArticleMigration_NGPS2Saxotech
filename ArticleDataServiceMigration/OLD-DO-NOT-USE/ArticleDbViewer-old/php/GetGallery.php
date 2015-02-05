<html>
<head>
<title>PHP Test</title>
</head>
<body>
<?php 

require_once( "phpSmug.php" );
require_once( "config.php" );

try {
	$f = new phpSmug( "APIKey=$APIKey", "AppName=$AppName" );
	// Login With EmailAddress and Password
	$f->login( "EmailAddress=$Email", "Password=$pwd" );	
	$albumid = $_GET["albumid"];
	$albumkey = $_GET["albumkey"];
	$articleuid =  $_GET["articleuid"];
	$images = $f->images_get( "AlbumID={$albumid}", "AlbumKey={$albumkey}", "Heavy=1" );
	$images = ( $f->APIVer == "1.2.2" ) ? $images['Images'] : $images;
	// Display the thumbnails and link to the medium image for each image
	foreach ( $images as $image ) {
		echo '<a href="'.$image['MediumURL'].'"><img src="'.$image['TinyURL'].'" title="'.$image['Caption'].'" alt="'.$image['id'].'" /> </a>';
		// echo '<input type="checkbox" name="deleteimage" value="'.$image['id'].'_'.$albumid.'_'.$albumkey.'_'.$articleuid.'" />';
		echo '<input type="checkbox" name="deleteimage" value="'.$articleuid.'_'.$image['id'].'_'.$image['FileName'].'" />';

	}
   }
catch ( Exception $e ) {
	echo "{$e->getMessage()} (Error Code: {$e->getCode()})";
}



?>
</body>
</html>


