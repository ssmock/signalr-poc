(function ($) {
    var hub = $.connection.FileViewer;

    var fileArea = $("#fileArea");
    var fileUpdate = $("#fileUpdate");

    hub.client.UpdateText = gotContents;

    function init() {
        hub.server.getFileContents().done(gotContents);
    }

    function gotContents(text) {
        fileArea.html(text);
        fileUpdate.html("Updated at " + (new Date()).toString());
    }

    $.connection.hub.start().done(init);
})($);