document.querySelectorAll('.open-form-plan-ui').forEach(function (element) {
    element.addEventListener('click', function () {
        document.getElementById('form-plan-detail-ui').style.display = 'block';
    });
});
document.querySelectorAll('.open-form-plan-ui2').forEach(function (element) {
    element.addEventListener('click', function () {
        document.getElementById('form-plan-detail-ui2').style.display = 'block';
        const categoryId = this.getAttribute('data-category-id');
    });
});
document.querySelector('.plan-close-icon2').addEventListener('click', function () {
    document.getElementById('form-plan-detail-ui2').style.display = 'none';
});
document.querySelector('.plan-close-icon').addEventListener('click', function () {
    document.getElementById('form-plan-detail-ui').style.display = 'none';
});

var acc = document.getElementsByClassName("itinerary-card-title");
var i;

for (i = 0; i < acc.length; i++) {
    acc[i].addEventListener("click", function () {
        this.classList.toggle("active");
        this.parentElement.classList.toggle("active");

        var pannel = this.nextElementSibling;

        if (pannel.style.display === "block") {
            pannel.style.display = "none";
        } else {
            pannel.style.display = "block";
        }
    });
}

var originalDiv = document.querySelector('.card-item-inp-2').cloneNode(true);

document.getElementById('card-add-btn').addEventListener('click', function () {
    var clonedDiv = originalDiv.cloneNode(true);
    clonedDiv.querySelector('input').value = '';
    document.getElementById('card-form-add').appendChild(clonedDiv);

    addDeleteEvent(clonedDiv.querySelector('.card-delete-btn'));
});

function addDeleteEvent(deleteBtn) {
    deleteBtn.addEventListener('click', function () {
        deleteBtn.parentElement.remove();
    });
}

addDeleteEvent(document.querySelector('.card-delete-btn'));


function updateCurrencyRate(amount, rate) {
    return amount * rate;
}

function changeCurrency(currency) {
    fetch(`/User/GetExchangeRate?currencyCode=${currency}`)
        .then(response => response.json())
        .then(rate => {
            document.querySelectorAll('.plan-card-right-price-curent').forEach(function (element) {
                let baseAmount = parseFloat(element.getAttribute('data-base-amount'));
                element.innerText = (baseAmount * rate).toFixed(2) + ' ' + currency;
            });
        })
        .catch(error => console.error('Error fetching exchange rate:', error));
}

document.addEventListener("DOMContentLoaded", function () {
    let savedCurrency = localStorage.getItem('selectedCurrency') || 'USD';
    document.getElementById('add-currency').value = savedCurrency;
    changeCurrency(savedCurrency);
});
