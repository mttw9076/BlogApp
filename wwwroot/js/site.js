// -------------------------------
// Pobranie elementów czatu
// -------------------------------
const chatBtn = document.getElementById("chatToggle");
const chatPanel = document.getElementById("chatPanel");
const chatClose = document.getElementById("chatClose");
const sendMessageBtn = document.getElementById("sendMessage");
const chatInput = document.getElementById("chatMessage");
const messagesContainer = document.getElementById("messages");

// -------------------------------
// Funkcja dodająca dymek wiadomości
// -------------------------------
function addMessage(text, type) {
    if (!messagesContainer) return;

    const bubble = document.createElement("div");
    bubble.classList.add("message");
    bubble.classList.add(type === "sent" ? "sent" : "received");
    bubble.textContent = text;

    messagesContainer.appendChild(bubble);

    // Auto-scroll
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
}

// -------------------------------
// Obsługa wysyłania wiadomości
// -------------------------------
if (sendMessageBtn) {
    sendMessageBtn.addEventListener("click", () => {
        const text = chatInput.value.trim();
        if (text.length === 0) return;

        addMessage(text, "sent");
        chatInput.value = "";

        // Tu później dodamy wysyłanie przez SignalR
    });
}

// -------------------------------
// Otwieranie panelu czatu
// -------------------------------
if (chatBtn) {
    chatBtn.addEventListener("click", () => {
        chatPanel.style.display = "flex";

        // Testowa wiadomość tylko przy pierwszym otwarciu
        if (messagesContainer.children.length === 0) {
            addMessage("Cześć! Jak mogę pomóc?", "received");
        }
    });
}

// -------------------------------
// Zamykanie panelu czatu
// -------------------------------
if (chatClose) {
    chatClose.addEventListener("click", () => {
        chatPanel.style.display = "none";
    });
}
