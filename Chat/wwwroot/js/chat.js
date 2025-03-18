document.addEventListener("DOMContentLoaded", function () {
    var username = document.querySelector("p strong").textContent;
    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    connection.start()
        .then(() => console.log("Connected to SignalR"))
        .catch(err => console.error("Connection failed:", err));

    function sendMessage() {
        var message = document.getElementById("messageInput").value;
        if (message.trim() === "") return;

        connection.invoke("SendMessage", "General", username, message)
            .catch(err => console.error(err));

        document.getElementById("messageInput").value = "";
    }

    document.getElementById("mediaInput").addEventListener("change", function (event) {
        var file = event.target.files[0];
        if (!file) return;

        var formData = new FormData();
        formData.append("file", file);

        fetch("/Chat/UploadMedia", { method: "POST", body: formData })
            .then(response => response.json())
            .then(data => {
                connection.invoke("SendMessage", "General", username, "", data.url)
                    .catch(err => console.error(err));
            });
    });

    connection.on("ReceiveMessage", (user, message, mediaUrl) => {
        var msg = document.createElement("li");
        msg.classList.add("list-group-item");
        msg.innerHTML = `<strong>${user}:</strong> ${message}`;

        if (mediaUrl) {
            var img = document.createElement("img");
            img.src = mediaUrl;
            img.style.maxWidth = "200px";
            msg.appendChild(img);
        }

        document.getElementById("messagesList").appendChild(msg);
    });

    window.sendMessage = sendMessage;
});
