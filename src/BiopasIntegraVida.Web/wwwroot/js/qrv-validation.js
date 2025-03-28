var qrvValidations = (function () {
	const cadastreSe = function () {
		var cadastroPessoaSection = document.getElementById('cadastro-pessoa-section');
		var cadastroSenhaSection = document.getElementById('cadastro-senha-section');
		var confirmacaoSection = document.getElementById('confirmacao-section');

		var cadastroForm = document.getElementById('cadastro-form');
		var senhaForm = document.getElementById('senha-form');

		var showSection = function (element) {
			var allSections = [cadastroPessoaSection, cadastroSenhaSection, confirmacaoSection];

			for (var section of allSections) {
				if (section === element) {
					qrvLib.replaceClasses(section, [], ["hidden"]);
				} else {
					qrvLib.replaceClasses(section, ["hidden"], []);
				}
			}
		}

		var submitAllForms = function (callback) {
			let formData = new FormData(cadastroForm);
			let formDataPrecios = new FormData(senhaForm);
			for (var pair of formDataPrecios.entries()) {
				formData.append(pair[0], pair[1]);
			}

			fetch("/cadastre-se/submit", {
				method: 'POST',
				body: formData
			})
				.then(response => callback(response.ok))
		}


		{
			var btnVoltarParaCadastro = document.getElementById('btn-voltar-para-cadastro');
			btnVoltarParaCadastro.addEventListener('click', function (event) {
				event.preventDefault();
				showSection(cadastroPessoaSection);
			});
		}

		(function () {
			// Masks
			var nascimentoField = cadastroForm.querySelector("#nascimento");
			qrvLib.maskInput(nascimentoField, "##/##/####");

			var dddField = cadastroForm.querySelector("#ddd");
			qrvLib.maskInput(dddField, "##");

			var crmField = cadastroForm.querySelector("#crm");
			qrvLib.maskInput(crmField, "######");

			// Form submit
			var submit = cadastroForm.querySelector('#submit-cadastro');
			var submitState = qrvLib.createToggleButtonState(submit, "Continuar", "Cadastrando...");

			var cpfValidationField = cadastroForm.querySelector('#cpf-validation');

			var blockSubmit = true;
			var cpfValidationField = cadastroForm.querySelector("#cpf-validation");
			var medicamentoValidationField = cadastroForm.querySelector("#medicamento-validation");
			// var tipoCadastroValidationField = cadastroForm.querySelector("#tipo-cadastro-validation");
			var nomeValidationField = cadastroForm.querySelector("#nome-validation");
			var nascimentoValidationField = cadastroForm.querySelector("#nascimento-validation");
			var emailValidationField = cadastroForm.querySelector("#email-validation");
			var dddValidationField = cadastroForm.querySelector("#ddd-validation");
			var telefoneValidationField = cadastroForm.querySelector("#telefone-validation");
			var crmValidationField = cadastroForm.querySelector("#crm-validation");
			var ufValidationField = cadastroForm.querySelector("#uf-validation");
			var comunicacaoCelularValidationField = cadastroForm.querySelector("#comunicacao-celular-validation");
			var comunicacaoEmailValidationField = cadastroForm.querySelector("#comunicacao-email-validation");

			cadastroForm.addEventListener('submit', function (event) {
				submitState.disable();

				event.preventDefault(); // Impede o envio padr�o do formul�rio

				qrvLib.cleanValidations(
					cpfValidationField,
					medicamentoValidationField,
					// tipoCadastroValidationField,
					nomeValidationField,
					nascimentoValidationField,
					emailValidationField,
					dddValidationField,
					telefoneValidationField,
					crmValidationField,
					ufValidationField,
					comunicacaoCelularValidationField,
					comunicacaoEmailValidationField
				);

				const formData = new FormData(cadastroForm);
				const actionUrl = cadastroForm.action;

				fetch("/cadastre-se/validation", {
					method: 'POST',
					body: formData
				})
					.then(response => response.json())
					.then(json => {
						if (!json.hasMessage) {
							// showSection(cadastroSenhaSection);

							submitAllForms((success) => {
								if (!success) {
									submitState.enable();
									alert('Ocorreu um erro')
								} else {
									showSection(confirmacaoSection);
								}
							});

							return;
						}

						qrvLib.toggleValidation(cpfValidationField, json.validations["cpf"]);
						qrvLib.toggleValidation(medicamentoValidationField, json.validations["medicamento"]);
						// qrvLib.toggleValidation(tipoCadastroValidationField, json.validations["tipo-cadastro"]);
						qrvLib.toggleValidation(nomeValidationField, json.validations["nome"]);
						qrvLib.toggleValidation(nascimentoValidationField, json.validations["nascimento"]);
						qrvLib.toggleValidation(emailValidationField, json.validations["email"]);
						qrvLib.toggleValidation(dddValidationField, json.validations["ddd"]);
						qrvLib.toggleValidation(telefoneValidationField, json.validations["telefone"]);
						qrvLib.toggleValidation(crmValidationField, json.validations["crm"]);
						qrvLib.toggleValidation(ufValidationField, json.validations["uf"]);
						qrvLib.toggleValidation(comunicacaoCelularValidationField, json.validations["comunicacao-celular"]);
						qrvLib.toggleValidation(comunicacaoEmailValidationField, json.validations["comunicacao-email"]);

						submitState.enable();
					})
					.catch(error => {
						console.error('Erro:', error);
					});
			});
		})();

		(function () {
			var classesOk = ['text-success', 'bi-check-circle-fill'];
			var classesError = ['text-danger', 'bi-exclamation-circle-fill'];
			var classesUnchecked = ['text-black-50', 'bi-circle-fill'];

			// Form submit
			var blockSubmit = true;
			var submit = senhaForm.querySelector('#submit-senha');
			var submitState = qrvLib.createToggleButtonState(submit, "Finalizar Cadastro", "Finalizando...");

			// validation labels
			var passValidationCharCount = senhaForm.querySelector('#password-validation-charcount');
			var passValidationSpecialChar = senhaForm.querySelector('#password-validation-specialchar');
			var passValidationNumber = senhaForm.querySelector('#password-validation-number');
			var passValidationMatch = senhaForm.querySelector('#password-validation-match');

			var displayValidationResult = function (element, classes) {
				var variations = [classesOk, classesError, classesUnchecked];
				qrvLib.addClasses(element, classes);

				for (var variation of variations) {
					if (variation !== classes) {
						qrvLib.removeClasses(element, variation);
					}
				}
			}

			senhaForm.addEventListener('submit', function (event) {
				submitState.disable();

				if (!blockSubmit)
					return true;

				event.preventDefault(); // Impede o envio padr�o do formul�rio

				displayValidationResult(passValidationCharCount, classesUnchecked);
				displayValidationResult(passValidationSpecialChar, classesUnchecked);
				displayValidationResult(passValidationNumber, classesUnchecked);
				displayValidationResult(passValidationMatch, classesUnchecked);

				const formData = new FormData(senhaForm);
				const actionUrl = senhaForm.action;

				fetch("/cadastre-se/password-validation", {
					method: 'POST',
					body: formData
				})
					.then(response => response.json())
					.then(json => {
						displayValidationResult(passValidationCharCount, json.charCountIsValid ? classesOk : classesError);
						displayValidationResult(passValidationSpecialChar, json.specialCharIsValid ? classesOk : classesError);
						displayValidationResult(passValidationNumber, json.numberIsValid ? classesOk : classesError);
						displayValidationResult(passValidationMatch, json.matchIsValid ? classesOk : classesError);

						if (!json.allValid) {
							submitState.enable();
							return;
						}

						submitAllForms((success) => {
							if (!success) {
								submitState.enable();
								alert('Ocorreu um erro')
							} else {
								showSection(confirmacaoSection);
							}
						});
					})
					.catch(error => {
						console.error('Erro:', error);
					});
			});
		})();
	};

	return {
		cadastreSe
	}
})();