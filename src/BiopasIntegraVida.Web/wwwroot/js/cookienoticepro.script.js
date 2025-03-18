/*
 * Copyright (c) 2021 Flerosoft (https://flerosoft.com)
 * Software Name: Cookie Notice Pro - jQuery Plugin
 * Product Page : https://cookienoticepro.flerosoft.com
 * Documentation: https://cookienoticepro.flerosoft.com/docs
 * Description: Cookie Notice Pro, a lightweight jQuery plugin, helps you to comply with GDPR.
Make your own cookie information popup in minutes.
 */

/*
*--------------------------------------------------------------------------
* CONFIG OR SETTINGS - Customize the popup banner
*--------------------------------------------------------------------------
*/
const config = {
	themeSettings: {
	  primaryColor: "#115cfa", // Primary Color of Popup Banner
	  darkColor: "#3b3e4a", // Dark Theme Color
	  lightColor: "#ffffff", // Light Theme Color
	  themeMode: "light", // Theme Mode (light|dark)
	},
  
	showSettingsBtn: false, // Hide or show the preference settings(true|false)
	showCloseIcon: false, // Hide or show the popup close icon(true|false)
	showDeclineBtn: true, // Hide or show the cookie decline button(true|false)
	fullWidth: false, // Full width popup works only when "displayPosition" is set to top/bottom
  
	displayPosition: "left", // Where popup should appear(top|right|bottom|left)
	settingsBtnLabel: "Customizar", // Text of settings button
  
	delay: 2000, // After how much time should popup appear(2000 is equal to 2 seconds)
	expires: 365, // Expiry date of cookie(365 is equal to 365 days)
  
	title: "Política de Privacidade e Cookies", // Title of popup bannner
	description: "Este site utiliza cookies ou tecnologias similares, para aprimorar sua experiência de navegação e oferecer recomendações personalizadas. Ao continuar utilizando este site, você concorda com a nossa ", // Message
	acceptBtnLabel: "Aceitar", // Accept cookie button text
	declineInfoBtnLabel: "Recusar", // Decline cookie button text
	moreInfoBtnLink: "#privacy-statement.html", // Learn more link(default: privacy policy page)
	moreInfoBtnLabel: "Política de Privacidade", // More info link text
	cookieTypesTitle: "Selecione os cookies para aceitar", // Title of cookie preference options
	necessaryCookieTypeLabel: "Necessário", // Label text of Necessary cookie item
	necessaryCookieTypeDesc: "Estes cookies são necessários para o funcionamento do site e não podem ser desativados nos nossos sistemas.", // Hover text of necessary cookies
	onConsentAccept: () => { // It will inject scripts in head if cookie preferences menu(showSettingsBtn) is enabled
		qrvCookie.checkActivateAnalytics();
	},
	onConsentReject: ()=> { // This code will run on cookie reject/decline
	  // console.log("Política Rejeitada!");
	},
	cookieTypes: [
	  // Cookie types, value and description (Cookie Preferences Selection)
	  {
		type: "Preferences",
		value: "preferences",
		description: "Preference cookies enable a website to remember information that changes the way the website behaves or looks, like your preferred language or the region that you are in.",
	  },
	  {
		type: "Marketing",
		value: "marketing",
		description: "Marketing cookies are used to track visitors across websites. The intention is to display ads that are relevant and engaging for the individual user and thereby more valuable for publishers and third party advertisers.",
	  },
	  {
		type: "Analytics",
		value: "analytics",
		description: "Analytics cookies allow us to count visits and traffic sources, so we can measure and improve the performance of our site. They help us know which pages are the most and least popular and see how visitors move around the site.",
	  },
	],
};



(function ($) {
	$.fn.cookieNoticePro = (event) => {
		changeRootVariables();
		let cookieConsentExists = cookieExists("cookieConsent");
		let cookiePrefsValue = accessCookie("cookieConsentPrefs");

		// If consent is not accepted
		if (!cookieConsentExists || event == "open") {
			$("#cookieNoticePro").remove();

			let cookieTypes='<li><input type="checkbox" name="gdprPrefItem" value="necessary" checked="checked" disabled="disabled" data-compulsory="on"> <label title="'+config.necessaryCookieTypeDesc+'">'+config.necessaryCookieTypeLabel+"</label></li>";

			preferences = JSON.parse(cookiePrefsValue);
			$.each(config.cookieTypes, (index, field) => {
				if (field.type !== "" && field.value !== "") {
					let cookieTypeDescription = "";
					if (field.description !== false) {
						cookieTypeDescription = ' title="' + field.description + '"';
					}
					cookieTypes+='<li><input type="checkbox" id="gdprPrefItem'+field.value+'" name="gdprPrefItem" value="'+field.value+'" data-compulsory="on"> <label for="gdprPrefItem'+field.value+'"'+cookieTypeDescription+">"+field.type+"</label></li>";
				}
			});

			let cookieNotice='<div id="cookieNoticePro" class="'+config.themeSettings.themeMode+" display-"+config.displayPosition+" full-width-"+config.fullWidth+'"><div id="closeIcon">'+ closeIcon +'</div><div class="title-wrap">'+cookieIcon+"<h4>"+config.title+'</h4></div><div class="content-wrap"><div class="msg-wrap"><p>'+config.description+' <a style="color:'+config.themeSettings.primaryColor+';" href="'+ config.moreInfoBtnLink +'">'+config.moreInfoBtnLabel+'</a></p><div id="cookieSettings">'+settingsIcon+config.settingsBtnLabel+'</div><div id="cookieTypes" style="display:none;"><h5>'+config.cookieTypesTitle+"</h5><ul>"+cookieTypes+'</ul></div></div><div class="btn-wrap"><button id="cookieAccept" style="color:'+config.themeSettings.lightColor+";background:"+config.themeSettings.primaryColor+";border: 1px solid "+config.themeSettings.primaryColor+';" type="button">'+config.acceptBtnLabel+'</button><button id="cookieReject" style="color:'+config.themeSettings.primaryColor+";border: 1px solid "+config.themeSettings.primaryColor+';" type="button">'+config.declineInfoBtnLabel+"</button></div>";

      setTimeout(() => {
        $("body").append(cookieNotice);
        $("#cookieNoticePro").hide().fadeIn("slow", () => {
          if(event == "open") {
            $("#cookieSettings").trigger("click");
            $.each(preferences, (index, field) => {
              $("input#gdprPrefItem" + field).prop("checked", true);
            });
          }
        });
        if(!config.showSettingsBtn) {
          $("#cookieSettings").hide();
        }
        if(!config.showDeclineBtn) {
          $("#cookieReject").hide();
        }
        if(!config.showCloseIcon) {
          $("#closeIcon").hide();
        }
      }, config.delay);
      $("body").on("click", "#cookieAccept", () => {
        hideCookieBanner(true, config.expires);
        $('input[name="gdprPrefItem"][data-compulsory="on"]').prop("checked", true);
        let prefs = [];
        $.each($('input[name="gdprPrefItem"]').serializeArray(), (i, field) => {
          prefs.push(field.value);
        });
        createCookie("cookieConsentPrefs", encodeURIComponent(JSON.stringify(prefs)), {
          expires: daysToUTC(365),
          path: "/"
        });
        config.onConsentAccept.call(this);
      });
      $("body").on("click", "#cookieSettings", () => {
        $('input[name="gdprPrefItem"]:not(:disabled)').attr("data-compulsory", "off").prop("checked", false);
        $("#cookieTypes").toggle("fast", () => {
          $("#cookieSettings").prop("disabled", false);
        });
      });
      $("body").on("click", "#closeIcon", () => {
        $("#cookieNoticePro").remove();
      });
      $("body").on("click", "#cookieReject", () => {
        hideCookieBanner(false, config.expires);
        config.onConsentReject.call(this);

		// Delete prefs cookie from brower on reject
		createCookie("cookieConsentPrefs", "", {
			expires: daysToUTC(-365),
			path: "/"
		});
      });
		}
		// If already consent is accepted, inject preferences
		else {
			config.showSettingsBtn? injectScripts() : null;
		}

	};

	/**
	 * Check if cookie exists
	 * @param {string} cookieName
	 */
	const cookieExists = (cookieName) => {
		if(document.cookie.indexOf(cookieName) > -1){
			return true;
		}
		return false;
	};

	/**
	 * Create the cookie and hide the banner
	 * @param {string} value
	 * @param {string} expiryDays
	 */
	const hideCookieBanner = (value, expiryDays) => {
		createCookie("cookieConsent", value, {
			expires: daysToUTC(expiryDays),
			path: "/",
		});
		$("#cookieNoticePro").fadeOut("fast", () => {
			$(this).remove();
		});
	};

	/**
	 * Set Cookie
	 * @param {string} name -  Cookie Name
	 * @param {string} value - Cookie Value
	 * @param {string} expiryDays - Expiry Date of cookie
	 */
	 var createCookie = (name, value, options={})=> {
        document.cookie = `${name}=${value}${
            Object.keys(options)
                .reduce((acc, key) => {
                    return acc + `;${key.replace(/([A-Z])/g, $1 => '-' + $1.toLowerCase())}=${
                        options[key]}`;
            }, '')
        }`;
    };

	/**
	 * Converts Days Into UTC String
	 * @param {number} days - Name of the cookie
	 * @return {string} UTC date string
	 */
	const daysToUTC = (days) => {
		const newDate = new Date();
		newDate.setTime(newDate.getTime() + days * 24 * 60 * 60 * 1000);
		return newDate.toUTCString();
	};

	/**
	 * Get Cookie By Name
	 * @param {string} name - Name of the cookie
	 * @return {string(number|Array)} Value of the cookie
	 */
	const accessCookie = (name) => {
		const cookies = document.cookie.split(";").reduce((acc, cookieString) => {
			const [key, value] = cookieString.split("=").map((s) => s.trim());
			if (key && value) {
				acc[key] = decodeURIComponent(value);
			}
			return acc;
		}, {});
		return name ? cookies[name] || false : cookies;
	};
	
	return window.cookieNoticePro = {
		init:()=>{
			$.fn.cookieNoticePro();
		},
		/**
		 * Reopens the cookie notice banner
		 */
		reinit:()=>{
			$.fn.cookieNoticePro("open");
		},

		/**
		 * Returns true if consent is given else false
		 */
		isAccepted: ()=>{
			let consent = accessCookie("cookieConsent");
			return JSON.parse(consent);
		},
	
		/**
		 * Returns the value of the cookieConsentPrefs cookie
		 */
		getPreferences: ()=>{
			let preferences = accessCookie("cookieConsentPrefs");
			return JSON.parse(preferences);
		},
	
		/**
		 * Check if a particular preference is accepted
		 * @param {string} cookieName
		 */
		isPreferenceAccepted: (cookieTypeValue)=>{
			let consent = accessCookie("cookieConsent");
			let preferences = accessCookie("cookieConsentPrefs");
			preferences = JSON.parse(preferences);
			if (consent === false) {
				return false;
			}
			if (preferences === false || preferences.indexOf(cookieTypeValue) === -1) {
				return false;
			}
			return true;
		}
	};
})(jQuery);


const changeRootVariables = () =>{
	$(':root').css('--cookieNoticeProLight', config.themeSettings.lightColor);
    $(':root').css('--cookieNoticeProDark', config.themeSettings.darkColor);
}

const settingsIcon =
	'<?xml version="1.0" ?><svg height="16px" version="1.1" viewBox="0 0 20 20" width="16px" xmlns="http://www.w3.org/2000/svg" xmlns:sketch="http://www.bohemiancoding.com/sketch/ns" xmlns:xlink="http://www.w3.org/1999/xlink"><title/><desc/><defs/><g fill="none" fill-rule="evenodd" id="Page-1" stroke="none" stroke-width="1"><g fill="#bfb9b9" id="Core" transform="translate(-464.000000, -380.000000)"><g id="settings" transform="translate(464.000000, 380.000000)"><path d="M17.4,11 C17.4,10.7 17.5,10.4 17.5,10 C17.5,9.6 17.5,9.3 17.4,9 L19.5,7.3 C19.7,7.1 19.7,6.9 19.6,6.7 L17.6,3.2 C17.5,3.1 17.3,3 17,3.1 L14.5,4.1 C14,3.7 13.4,3.4 12.8,3.1 L12.4,0.5 C12.5,0.2 12.2,0 12,0 L8,0 C7.8,0 7.5,0.2 7.5,0.4 L7.1,3.1 C6.5,3.3 6,3.7 5.4,4.1 L3,3.1 C2.7,3 2.5,3.1 2.3,3.3 L0.3,6.8 C0.2,6.9 0.3,7.2 0.5,7.4 L2.6,9 C2.6,9.3 2.5,9.6 2.5,10 C2.5,10.4 2.5,10.7 2.6,11 L0.5,12.7 C0.3,12.9 0.3,13.1 0.4,13.3 L2.4,16.8 C2.5,16.9 2.7,17 3,16.9 L5.5,15.9 C6,16.3 6.6,16.6 7.2,16.9 L7.6,19.5 C7.6,19.7 7.8,19.9 8.1,19.9 L12.1,19.9 C12.3,19.9 12.6,19.7 12.6,19.5 L13,16.9 C13.6,16.6 14.2,16.3 14.7,15.9 L17.2,16.9 C17.4,17 17.7,16.9 17.8,16.7 L19.8,13.2 C19.9,13 19.9,12.7 19.7,12.6 L17.4,11 L17.4,11 Z M10,13.5 C8.1,13.5 6.5,11.9 6.5,10 C6.5,8.1 8.1,6.5 10,6.5 C11.9,6.5 13.5,8.1 13.5,10 C13.5,11.9 11.9,13.5 10,13.5 L10,13.5 Z" id="Shape"/></g></g></g></svg>';

const cookieIcon =
	'<svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 40 40"> <g fill="none" fill-rule="evenodd"> <circle cx="20" cy="20" r="20" fill="#D5A150"></circle> <path fill="#AD712C" d="M32.44 4.34a19.914 19.914 0 0 1 4.34 12.44c0 11.046-8.954 20-20 20a19.914 19.914 0 0 1-12.44-4.34C8.004 37.046 13.657 40 20 40c11.046 0 20-8.954 20-20 0-6.343-2.954-11.996-7.56-15.66z"> </path> <path fill="#C98A2E" d="M10.903 11.35c-.412 0-.824-.157-1.139-.471a4.432 4.432 0 0 1 0-6.26 4.397 4.397 0 0 1 3.13-1.297c1.183 0 2.294.46 3.13 1.296a1.61 1.61 0 0 1-2.276 2.277 1.2 1.2 0 0 0-.854-.354 1.208 1.208 0 0 0-.854 2.06 1.61 1.61 0 0 1-1.137 2.749z"> </path> <circle cx="12.894" cy="7.749" r="2.817" fill="#674230"></circle> <path fill="#7A5436" d="M10.09 7.48l-.003.032a1.566 1.566 0 0 0 1.624 1.683 2.824 2.824 0 0 0 2.703-2.578 1.566 1.566 0 0 0-1.624-1.683 2.823 2.823 0 0 0-2.7 2.546z"> </path> <path fill="#C98A2E" d="M4.464 24.227c-.412 0-.824-.157-1.138-.471a4.432 4.432 0 0 1 0-6.26 4.398 4.398 0 0 1 3.13-1.297c1.182 0 2.294.46 3.13 1.297a1.61 1.61 0 0 1-2.277 2.276 1.2 1.2 0 0 0-.853-.353 1.208 1.208 0 0 0-.854 2.06 1.61 1.61 0 0 1-1.138 2.748z"> </path> <circle cx="6.456" cy="20.626" r="2.817" fill="#674230"></circle> <path fill="#7A5436" d="M3.651 20.356a1.566 1.566 0 0 0 1.62 1.716 2.824 2.824 0 0 0 2.703-2.578 1.566 1.566 0 0 0-1.622-1.683 2.824 2.824 0 0 0-2.7 2.546z"> </path> <path fill="#C98A2E" d="M10.098 32.276c-.412 0-.824-.158-1.138-.472a4.432 4.432 0 0 1 0-6.26 4.397 4.397 0 0 1 3.13-1.297c1.182 0 2.294.46 3.13 1.297a1.61 1.61 0 0 1-2.277 2.276 1.2 1.2 0 0 0-.853-.353 1.208 1.208 0 0 0-.854 2.06 1.61 1.61 0 0 1-1.138 2.749z"> </path> <circle cx="12.089" cy="28.674" r="2.817" fill="#674230"></circle> <path fill="#7A5436" d="M9.285 28.405a1.566 1.566 0 0 0 1.62 1.716 2.824 2.824 0 0 0 2.703-2.578 1.566 1.566 0 0 0-1.622-1.684 2.824 2.824 0 0 0-2.7 2.546z"> </path> <path fill="#C98A2E" d="M18.95 37.91c-.411 0-.823-.158-1.137-.472a4.432 4.432 0 0 1 0-6.26 4.397 4.397 0 0 1 3.13-1.297c1.182 0 2.294.46 3.13 1.297a1.61 1.61 0 0 1-2.277 2.276 1.2 1.2 0 0 0-.853-.353 1.208 1.208 0 0 0-.854 2.06 1.61 1.61 0 0 1-1.138 2.748z"> </path> <circle cx="20.942" cy="34.308" r="2.817" fill="#674230"></circle> <path fill="#7A5436" d="M18.138 34.038l-.002.033a1.566 1.566 0 0 0 1.623 1.684 2.824 2.824 0 0 0 2.703-2.578 1.566 1.566 0 0 0-1.623-1.684 2.824 2.824 0 0 0-2.7 2.546z"> </path> <path fill="#C98A2E" d="M20.56 15.385c-.411 0-.823-.157-1.138-.471a4.432 4.432 0 0 1 0-6.26 4.397 4.397 0 0 1 3.13-1.297c1.183 0 2.294.46 3.13 1.296a1.61 1.61 0 0 1-2.276 2.277 1.2 1.2 0 0 0-.854-.354 1.208 1.208 0 0 0-.854 2.06 1.61 1.61 0 0 1-1.137 2.75z"> </path> <circle cx="22.552" cy="11.784" r="2.817" fill="#674230"></circle> <path fill="#7A5436" d="M19.748 11.514l-.003.033a1.566 1.566 0 0 0 1.624 1.683 2.824 2.824 0 0 0 2.703-2.578 1.566 1.566 0 0 0-1.624-1.683 2.823 2.823 0 0 0-2.7 2.546z"> </path> <path fill="#C98A2E" d="M30.219 29.861c-.412 0-.824-.157-1.139-.471a4.432 4.432 0 0 1 0-6.26 4.397 4.397 0 0 1 3.13-1.297c1.183 0 2.294.46 3.13 1.296a1.61 1.61 0 0 1-2.276 2.277 1.2 1.2 0 0 0-.854-.354 1.208 1.208 0 0 0-.854 2.06 1.61 1.61 0 0 1-1.137 2.75z"> </path> <circle cx="32.21" cy="26.26" r="2.817" fill="#674230"></circle> <path fill="#7A5436" d="M29.406 25.99a1.566 1.566 0 0 0 1.62 1.716 2.824 2.824 0 0 0 2.703-2.578 1.566 1.566 0 0 0-1.623-1.683 2.824 2.824 0 0 0-2.7 2.546z"> </path> <path fill="#C98A2E" d="M29.414 14.57c-.412 0-.824-.158-1.139-.472a4.432 4.432 0 0 1 0-6.26 4.397 4.397 0 0 1 3.13-1.297c1.183 0 2.295.46 3.13 1.297a1.61 1.61 0 0 1-2.276 2.276 1.2 1.2 0 0 0-.853-.353 1.208 1.208 0 0 0-.854 2.06 1.61 1.61 0 0 1-1.138 2.748z"> </path> <circle cx="31.405" cy="10.968" r="2.817" fill="#674230"></circle> <path fill="#7A5436" d="M28.601 10.698a1.566 1.566 0 0 0 1.62 1.716 2.824 2.824 0 0 0 2.703-2.578 1.566 1.566 0 0 0-1.622-1.683 2.824 2.824 0 0 0-2.7 2.546z"> </path> <path fill="#C98A2E" d="M17.341 24.227c-.412 0-.824-.157-1.138-.471a4.432 4.432 0 0 1 0-6.26 4.397 4.397 0 0 1 3.13-1.297c1.183 0 2.294.46 3.13 1.297a1.61 1.61 0 0 1-2.276 2.276 1.2 1.2 0 0 0-.854-.354 1.208 1.208 0 0 0-.854 2.06 1.61 1.61 0 0 1-1.138 2.75z"> </path> <circle cx="19.333" cy="20.626" r="2.817" fill="#674230"></circle> <path fill="#7A5436" d="M16.529 20.356l-.003.033a1.566 1.566 0 0 0 1.623 1.684 2.824 2.824 0 0 0 2.703-2.578 1.566 1.566 0 0 0-1.623-1.684 2.824 2.824 0 0 0-2.7 2.546z"> </path> <g fill="#AD712C" transform="translate(2.656 1.875)"> <circle cx="7.485" cy="21.143" r="1"></circle> <circle cx="11.509" cy="21.143" r="1"></circle> <circle cx="9.497" cy="17.521" r="1"></circle> <circle cx="2.253" cy="24.765" r="1"></circle> <circle cx="10.301" cy="33.618" r="1"></circle> <circle cx="12.716" cy="30.399" r="1"></circle> <circle cx="16.74" cy="25.57" r="1"></circle> <circle cx="23.179" cy="23.155" r="1"></circle> <circle cx="21.569" cy="24.765" r="1"></circle> <circle cx="23.984" cy="27.179" r="1"></circle> <circle cx="23.984" cy="32.008" r="1"></circle> <circle cx="32.837" cy="15.107" r="1"></circle> <circle cx="30.422" cy="31.203" r="1"></circle> <circle cx="18.35" cy=".62" r="1"></circle> <circle cx="3.863" cy="7.863" r="1"></circle> <circle cx=".644" cy="12.692" r="1"></circle> <circle cx="9.899" cy="13.9" r="1"></circle> <circle cx="12.314" cy="12.692" r="1"></circle> <circle cx="9.899" cy="11.485" r="1"></circle> <circle cx="21.167" cy="17.521" r="1"></circle> <circle cx="15.935" cy="5.449" r="1"></circle> <circle cx="23.581" cy="12.692" r="1"></circle> <circle cx="24.788" cy="16.314" r="1"></circle> <circle cx="27.203" cy="16.314" r="1"></circle> <circle cx="27.203" cy="18.729" r="1"></circle> <circle cx="22.776" cy="4.242" r="1"></circle> <circle cx="25.191" cy="3.034" r="1"></circle> </g> </g></svg>';

	const closeIcon = '<?xml version="1.0" ?><svg viewBox="0 0 96 96" xmlns="http://www.w3.org/2000/svg"><title/><g fill="#bfb9b9"><path d="M48,0A48,48,0,1,0,96,48,48.0512,48.0512,0,0,0,48,0Zm0,84A36,36,0,1,1,84,48,36.0393,36.0393,0,0,1,48,84Z"/><path d="M64.2422,31.7578a5.9979,5.9979,0,0,0-8.4844,0L48,39.5156l-7.7578-7.7578a5.9994,5.9994,0,0,0-8.4844,8.4844L39.5156,48l-7.7578,7.7578a5.9994,5.9994,0,1,0,8.4844,8.4844L48,56.4844l7.7578,7.7578a5.9994,5.9994,0,0,0,8.4844-8.4844L56.4844,48l7.7578-7.7578A5.9979,5.9979,0,0,0,64.2422,31.7578Z"/></g></svg>';