const qrvCookie = (function () {

    /*
    // Example: Google Adwords cookie, DoubleClick, Remarketing pixels, Social Media cookies
    if (cookieNoticePro.isPreferenceAccepted("marketing") === true) {
            
    }

    // Example: Remember password, language, etc
    if (cookieNoticePro.isPreferenceAccepted("preferences") === true) {
        console.log("Preferences Scripts Running....");
    }
    */

    let gCode = null;

    var checkActivateAnalytics = function () {
        if (cookieNoticePro.isPreferenceAccepted("analytics") === true) {
            var s = document.createElement('script');
            s.setAttribute('src', `https://www.googletagmanager.com/gtag/js?id=${gCode}`);
            s.async = true;
            document.body.appendChild(s);

            window.dataLayer = window.dataLayer || [];
            function gtag() { dataLayer.push(arguments); }
            gtag('js', new Date());
            gtag('config', gCode);
        }
    }

    var init = function (googleAnalyticsCode) {
        gCode = googleAnalyticsCode;

        $(document).ready(function () {
            cookieNoticePro.init();
        });

        checkActivateAnalytics();
    }

    return {
        init,
        checkActivateAnalytics
    };
})();