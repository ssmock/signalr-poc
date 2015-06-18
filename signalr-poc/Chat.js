(function () {
    var userName = prompt("Who are you?");

    var chat = $.connection.Chat;

    var messageList = $("#messageList");
    var messageIn = $("#messageIn");

    $("#message").submit(function (e) {
        e.preventDefault();

        var text = messageIn.val();

        if (text) {
            chat.server.say({
                UserName: userName,
                Text: messageIn.val()
            });

            messageIn.val("");
        }
    });

    chat.client.AcceptNewMessage = function (message) {
        var message = "<div>"
            + "<div>" + message.UserName + "</div>"
            + "<div>" + message.Text + "</div>"
        + "</div>";

        messageList.append(message);
    };

    $.connection.hub.start();
})($);