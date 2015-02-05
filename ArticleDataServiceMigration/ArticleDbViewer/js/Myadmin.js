

        function OpenWindow(target) {
            myWindow= window.open( target, ' ','scrollbars=yes,toolbar=no,menubar=no,width=1000,height=800')
        }
     
        function handleChange(cb, article_uid,asset_uid) {
            var vflag = (cb.checked) ? "Y" : "N";
            HttpActionPost('MyCheckbox.aspx?article_uid=' + article_uid + '&asset_uid=' + asset_uid + '&checked=' + vflag);
        }

        function ShowModal(target,width,height) {
            var retValue = window.showModalDialog(target, null, "dialogwidth:" + width + "px;dialogheight:" + height + "px;resizable=1;scroll:1");

        }



        function parseArugments(searchString) {
             rndflag = 0;
             randomNumber = Math.floor(Math.random() * 100000);
             idx = searchString.lastIndexOf('?');
            //  alert('idx: ' + idx);
             if ( idx <= 0) { // no Arguments 
                 searchString = searchString + '?rndom=' + randomNumber;
          //       alert('No arguments return:' + searchString );
                 return searchString;
             }
             searchString = searchString.substring(idx+1);
             var nvPairs = searchString.split("&");
             var replaceString = "";
             for (i = 0; i < nvPairs.length; i++)
             {
                var nvPair = nvPairs[i].split("=");
                var name = nvPair[0];
                var value = nvPair[1];
            //    alert('name: ' + name + ' value: ' + value);
                if (name == "rndom") {
                    rndflag = 1;
                    value = randomNumber;
                }
                replaceString = replaceString.concat(name + '=' + value);
              //   alert('replaceString: ' + replaceString);
                if (i < nvPairs.length - 1) {
                    replaceString = replaceString.concat('&');
                }
             } // end for
             if (rndflag == 0) {
                 replaceString = replaceString.concat('&rndom=' + randomNumber);
             }
         //    alert(replaceString);
             return '?' + replaceString;
         }

   
    function HttpActionPost(url, args) {
        var xmlHttp;

        var status = false;
        try {
            // Firefox, Opera 8 , Safari
            xmlHttp = new XMLHttpRequest();
            status = true;
        } catch (e) { }
        if (status == false) {
            try {  // Internet Explorer
                xmlHttp = new ActiveXObject("Msxml2.XMLHTTP");
                status = true;
            } catch (e) { }
        }
        if (status == false) {
            try {  // Internet Explorer
                xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
                status = true;
            } catch (e) { return false; }
        }
        xmlHttp.onreadystatechange = function() {
            if (xmlHttp.readyState == 4) { // request is complete
                //  answer = xmlHttp.responseText;
                //  alert(answer);
            }
        }  // onreadystatechange

        xmlHttp.open("POST", url, false);
        xmlHttp.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xmlHttp.send(args);
        strResult = xmlHttp.responseText;
        strwindow = window.location.href
   //     alert('Before parse: ' + strwindow);
        mylocation = parseArugments(strwindow);
   //     alert('After parse: ' + mylocation);
        window.location = mylocation;

    }
