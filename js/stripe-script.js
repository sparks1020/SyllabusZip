var createCheckoutSession = function (priceId, selection) {
  return fetch("/create-checkout-session", {
    method: "POST",
    credentials: "same-origin",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      priceId: priceId,
      selection: selection,
    }),
  }).then(function (result) {
    return result.json();
  });
};

var stripe = Stripe(stripePK);

document.getElementById("checkout").addEventListener("click", function (evt) {
  var selection = document.querySelector("[name=btnradio]:checked").value;
  var PriceId = PriceIdMonth;
  if (selection == "semester") {
    PriceId = PriceIdSemester;
  }
  createCheckoutSession(PriceId, selection).then(function (data) {
    // Call Stripe.js method to redirect to the new Checkout page
    stripe
      .redirectToCheckout({
        sessionId: data.sessionId,
      })
      .then(handleResult);
  });
});
