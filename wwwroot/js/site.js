// Funkcja obsługująca kliknięcie w przycisk toggle
function toggleButton(element) {
    var button = $(element);
    var isOn = button.hasClass("toggle-button-true"); // Sprawdzamy aktualny stan
    var newState = !isOn; // Odwracamy stan

    var url = button.data('url'); // Pobieramy URL z atrybutu "data-url"
    var index = button.data('index'); // Pobieramy indeks z atrybutu "data-index"

    if (!url) {
        console.error("❌ Brak adresu URL w 'data-url'");
        alert("Wystąpił błąd: brak adresu URL w konfiguracji.");
        return;
    }

    if (index === undefined) {
        console.error("❌ Brak indeksu w 'data-index'");
        alert("Wystąpił błąd: brak indeksu przycisku.");
        return;
    }

    // Pobranie ikon z atrybutów data
    var icon = button.find("i");
    var iconOn = button.data("icon-on") || "fa-toggle-on";  // Domyślna ikona
    var iconOff = button.data("icon-off") || "fa-toggle-off";

    // Wysyłamy żądanie AJAX do serwera
    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ index: index, state: newState })
    })
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => {
                    throw new Error(`Błąd HTTP ${response.status}: ${text}`);
                });
            }
            return response.text(); // Obsługa pustej odpowiedzi
        })
        .then(text => text ? JSON.parse(text) : {}) // Obsługa pustego body
        .then(data => {
            console.log('✅ Zmieniono stan.', data);

            // Zmieniamy klasy ON/OFF
            button.toggleClass("toggle-button-true toggle-button-false");

            // Zmiana ikony w zależności od nowego stanu
            icon.removeClass().addClass("fas " + (newState ? iconOn : iconOff));
        })
        .catch(error => {
            console.error('❌ Błąd zmiany stanu:', error);
            alert(`Wystąpił błąd: ${error.message}. Spróbuj ponownie.`);
        });
}

// Dodajemy nasłuchiwanie zdarzeń po załadowaniu strony
$(document).ready(function () {
    $(".toggle-button").click(function () {
        toggleButton(this);
    });
});
