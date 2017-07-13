/**
 * Created by hongh on 2017/7/13.
 */
/**
 * Created by hongh on 2017/7/10.
 */
$(document).ready(function(){
    $("#submitBt").click(function(){
        $.ajax({
            type: "POST",
            url:"/User/SubmitTask",
            datatype:"text",
            data:$('#submitBt').value,
            error: function(request) {
                alert("Connection error");
            },
            success: function(data) {
                $("#submitBt").attr('disabled',true);
                $("#submitBt").html("Submitted");
                $("#submitBt").removeClass("uk-button-primary");
            }
        })
    })

    $("#checkBt").click(function(){
        $.ajax({
            type: "POST",
            url:"/User/CheckTask",
            atatype:"text",
            data:$('#checkBt').value,
            error: function(request) {
                alert("Connection error");
            },
            success: function(data) {
                $("#checkBt").attr('disabled',true);
                $("#checkBt").html("Done");
                $("#checkBt").removeClass("uk-button-primary");
            }
        })
    })
})
