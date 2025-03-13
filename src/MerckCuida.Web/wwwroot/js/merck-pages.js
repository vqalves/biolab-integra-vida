var merckPages = (function () {

    const indexPage = function (googleMapsKey) {
        (g => { var h, a, k, p = "The Google Maps JavaScript API", c = "google", l = "importLibrary", q = "__ib__", m = document, b = window; b = b[c] || (b[c] = {}); var d = b.maps || (b.maps = {}), r = new Set, e = new URLSearchParams, u = () => h || (h = new Promise(async (f, n) => { await (a = m.createElement("script")); e.set("libraries", [...r] + ""); for (k in g) e.set(k.replace(/[A-Z]/g, t => "_" + t[0].toLowerCase()), g[k]); e.set("callback", c + ".maps." + q); a.src = `https://maps.${c}apis.com/maps/api/js?` + e; d[q] = f; a.onerror = () => h = n(Error(p + " could not load.")); a.nonce = m.querySelector("script[nonce]")?.nonce || ""; m.head.append(a) })); d[l] ? console.warn(p + " only loads once. Ignoring:", g) : d[l] = (f, ...n) => r.add(f) && u().then(() => d[l](f, ...n)) })({
            key: googleMapsKey,
            v: "weekly",
            // Use the 'v' parameter to indicate the version to use (weekly, beta, alpha, etc.).
            // Add other bootstrap parameters as needed, using camel case.
        });

        var gmaps = (function () {
            let map;

            async function initMap() {
                const { Map } = await google.maps.importLibrary("maps");

                map = new Map(document.getElementById("google-maps-container"), {
                    center: { lat: -34.397, lng: 150.644 },
                    zoom: 13,
                    mapId: 'merckcuida-farmacias'
                });
            }

            var displayLojas = async function (lojas) {
                const { AdvancedMarkerElement } = await google.maps.importLibrary("marker");
                const mapList = [map];

                for (var mapItem of mapList) {
                    if (!mapItem)
                        continue;

                    // Recenter map
                    const center = new google.maps.LatLng(lojas[0].latitude, lojas[0].longitude);
                    mapItem.panTo(center);

                    // Add markers
                    for (var loja of lojas) {
                        try {
                            const marker = new AdvancedMarkerElement({
                                map: mapItem,
                                position: { lat: loja.latitude, lng: loja.longitude },
                            });
                        }
                        catch (err) {
                            console.log(err);
                        }
                    }
                }
            }

            initMap();

            return {
                displayLojas
            }
        })();

		// Cadastre-se
		(function () {
			var form = document.getElementById('cadastre-se-form');

			// Masks
			var cpfField = form.querySelector('#input-cpf');
			merckLib.maskInput(cpfField, "###.###.###-##");

			// Form submit
			var blockSubmit = true;
			var submit = form.querySelector('#cadastre-se-submit');
			var submitState = merckLib.createToggleButtonState(submit, "Cadastre-se", "Cadastrando...");

			var cpfValidationField = form.querySelector('#cpf-validation');

			form.addEventListener('submit', function (event) {
				submitState.disable();

				if (!blockSubmit)
					return true;

				event.preventDefault(); // Impede o envio padrão do formulário
				merckLib.toggleValidation(cpfValidationField, []);

				const formData = new FormData(form);
				const actionUrl = form.action;

				fetch("/index/cadastre-se-validation", {
					method: 'POST',
					body: formData
				})
					.then(response => response.json())
					.then(json => {
						submitState.enable();
						merckLib.toggleValidation(cpfValidationField, json.validations['cpf']);

						if (!json.hasMessage) {
							merckLib.openRegulamento(true, (event) => {
								var button = event.target;
								var regulamentoAceitarState = merckLib.createToggleButtonState(button, "Aceitar e continuar", "Aceitando...");

								event.preventDefault();
								blockSubmit = false;

								merckLib.removeClasses(button, ['text-white']);

								regulamentoAceitarState.disable();
								form.submit();
							});
						}
					})
					.catch(error => {
						console.error('Erro:', error);
					});
			});
		})();

		// Busca por CEP
		(function () {
			var formDesktop = document.getElementById('busca-cep-form');
			var containerParent = document.querySelectorAll('.box-resultados');
			var containers = document.querySelectorAll('.box-resultados-int');

			var createItem = function (data) {
				var phoneHtml = "";
				if (data.phone) {
					phoneHtml = `<div class="resultado-telefone">
						<i class="i-style rounded bi-telephone" style="background-color:#FF1493"></i>
						<span class="text-uppercase">${data.phone}</span>
					</div>`;
				}

				var html = `<div class="resultado-pesquisa">
					<div class="resultado-nome">${data.name} ${data.zipCode}</div>
					<div class="resultado-endereco">
						<i class="i-style rounded uil-map-marker-alt" style="background-color:#FF1493"></i>
						<span class="text-uppercase">${data.address}</span>
					</div>
					${phoneHtml}
				</div>`;

				return html;
			}

			var setupForm = function (form) {
				var submit = form.querySelector('.submit-cep');
				var submitState = merckLib.createToggleButtonState(submit, "Buscar", "Buscando...");

				form.addEventListener('submit', function (event) {
					submitState.disable();

					event.preventDefault(); // Impede o envio padrão do formulário

					const formData = new FormData(form);

					fetch("/index/locais-por-cep", {
						method: 'POST',
						body: formData
					})
						.then(response => response.json())
						.then(json => {
							var html = json.map(x => createItem(x)).join('');

							for (var container of containers)
								container.innerHTML = html;

							if (json && json.length > 0) {
								gmaps.displayLojas(json);
								merckLib.removeClasses(containerParent, ['hidden']);
							} else {
								merckLib.addClasses(containerParent, ['hidden']);
							}
						})
						.catch(error => {
							console.error('Erro: ', error);
						})
						.finally(() => {
							submitState.enable();
						});
				});
			}

			// Form submit
			for (var form of [formDesktop]) {
				setupForm(form);
			}
		})();
    }

    return {
        indexPage
    }
})();