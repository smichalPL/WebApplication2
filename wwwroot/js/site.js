// Funkcja obsługująca kliknięcie w przycisk toggle
function toggleButton(element) {
    var button = $(element);
    var isOn = button.hasClass("toggle-button-true"); // Sprawdzamy aktualny stan
    var newState = !isOn; // Odwracamy stan

    var url = button.data('url'); // Pobieramy URL z atrybutu "data-url"
    var index = button.data('index'); // Pobieramy indeks z atrybutu "data-index"

    // Pobranie ikon z atrybutów data
    var icon = button.find("i");
    var iconOn = button.data("icon-on");
    var iconOff = button.data("icon-off");

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
                return response.json().then(errorData => {
                    throw new Error(errorData.error || `Błąd HTTP! status: ${response.status}`);
                });
            }
            return response.json();
        })
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
