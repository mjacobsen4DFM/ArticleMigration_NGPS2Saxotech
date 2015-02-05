$(function () {
    $("#migratepub").click(function (e) {
        mystart = $('.mystart').val();
        myend = $('.myend').val();
        mypub = $('.mypub').val();
        $('.mytarget').html("<iframe src='migrateqa0.aspx?mystart=" + mystart + "&myend=" + myend + "&mypub=" + mypub + "</iframe>");
    }); // migratepub
});     // end function