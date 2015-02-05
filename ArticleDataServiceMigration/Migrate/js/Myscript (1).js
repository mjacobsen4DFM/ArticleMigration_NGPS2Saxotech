function process() {

    //    disableButton(document.aspnetForm.processButton);
    try {
        this.document.forms[0].action = "default.aspx?process=YES";
        this.document.forms[0].target = "view";
        this.document.forms[0].submit();
    } catch (e) {
        alert(e);
    }
} // process
