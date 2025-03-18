var qrvLib = (function() {
    var debug = true;

    var sendDebug = function(...params) {
        if(debug) {
            console.log(params);
        }
    }

    var openRegulamento = function (showButton, onClick) {
        var regulamentoModal = document.getElementById('modal-regulamento');
        var container = document.getElementById('container-aceitar-lgpd');
        var button = container.querySelector('button');

        if (showButton) {
            removeClasses(container, ['hidden']);
            button.onclick = onClick;
        } else {
            addClasses(container, ['hidden']);
        }

        jQuery.magnificPopup.open({
            items: {
                src: regulamentoModal,
                type: 'inline'
            }
        });
    }

    var maskInput = function(input, pattern) {
        input.addEventListener('input', function() {
            var value = input.value.replace(/\D/g, ''); // Remove non-digit characters
            var newValue = '';
            var patternIndex = 0;
            var valueIndex = 0;

            while (patternIndex < pattern.length && valueIndex < value.length) {
                if (pattern[patternIndex] === '#') {
                    newValue += value[valueIndex++];
                } else {
                    newValue += pattern[patternIndex];
                }
                patternIndex++;
            }

            input.value = newValue;
        });
    }

    var createToggleButtonState = function(target, enabledText, disabledText) {
        var disable = function () {
            var arr = convertArray(target);

            for (var item of arr) {
                sendDebug("Disabling button", item, disabledText);

                if (item.tagName === "INPUT")
                    item.value = disabledText;
                else
                    item.innerHTML = disabledText;
                    
                item.disabled = true;
            }
        }

        var enable = function () {
            var arr = convertArray(target);

            for (var item of arr) {
                sendDebug("Enabling button", item, enabledText);

                if (item.tagName === "INPUT")
                    item.value = enabledText;
                else
                    item.innerHTML = enabledText;

                item.disabled = false;
            }
        }

        return {
            enable,
            disable
        }
    }

    var convertArray = function (element) {
        var arr;

        if (NodeList.prototype.isPrototypeOf(element)) {
            arr = element;
        } else {
            arr = [element];
        }

        return arr;
    }

    var cleanValidations = function(...targets) {
        for(var target of targets)
            toggleValidation(target, []);
    }

    var removeClasses = function (elements, classes) {
        var arr = convertArray(elements);

        for (var element of arr)
            for(var c of classes)
                while (element.classList.contains(c))
                    element.classList.remove(c);
    }

    var addClasses = function (elements, classes) {
        var arr = convertArray(elements);

        for(var element of arr)
            for(var c of classes)
                while (!element.classList.contains(c))
                    element.classList.add(c);
    }

    var replaceClasses = function(element, classesToAdd, classesToRemove) {
        addClasses(element, classesToAdd);
        removeClasses(element, classesToRemove);
    }

    var toggleValidation = function(target, validations) {
        sendDebug("Displaying validations", target, validations);

        var display = validations && validations.length > 0;

        while(!display && !target.classList.contains('hidden'))
            target.classList.add('hidden');
        
        validations = validations || [];
        var content = validations.join('\r\n');
        target.innerText = content;

        while (display && target.classList.contains('hidden'))
            target.classList.remove('hidden');
    }

    var makeAutocomplete = function (element, items) {

        // Função para inicializar o autocomplete
        function autocomplete(inp, itemList) {
            let currentFocus;

            inp.addEventListener("focus", function (e) {
                refreshSuggestionList();
            });

            // Adiciona o evento 'input' no campo de texto
            inp.addEventListener("input", function (e) {
                refreshSuggestionList();
            });

            // Adiciona os eventos 'keydown' para navegação no dropdown
            inp.addEventListener("keydown", function (e) {
                let item = document.getElementsByClassName("autocomplete-items");

                if (item && item[0])
                    item = item[0].getElementsByTagName("div");
                else
                    return;

                if (e.keyCode === 40) { // seta para baixo
                    currentFocus++;
                    addActive(item);
                } else if (e.keyCode === 38) { // seta para cima
                    currentFocus--;
                    addActive(item);
                } else if (e.keyCode === 13) { // enter
                    e.preventDefault();

                    if (currentFocus > -1) {
                        if (item)
                            item[currentFocus].click();
                    }
                }
            });

            function refreshSuggestionList() {
                let val = element.value;
                var lowerCaseValue = val.toLowerCase();

                // Fecha a lista de sugestões, se já estiver aberta
                closeAllLists();
                currentFocus = -1;

                // Cria um elemento <div> para conter as sugestões
                var htmlItemContainer = document.createElement("DIV");
                htmlItemContainer.setAttribute("class", "autocomplete-items");
                element.parentNode.appendChild(htmlItemContainer);

                // Filtrar lista
                var listaFiltrada = [];

                for (var item of itemList) {
                    var lowerItem = item.toLowerCase();

                    if (val.length === 0 || lowerItem.includes(lowerCaseValue)) {
                        listaFiltrada.push(item);
                    }
                }

                // Percorre todos os itens do array e cria uma sugestão se houver correspondência
                for (var item of listaFiltrada) {
                    var htmlItem = document.createElement("DIV");

                    if (val.length === 0) {
                        htmlItem.innerHTML = item;
                    } else {
                        htmlItem.innerHTML = highlightSubstring(item, val);
                    }

                    htmlItem.addEventListener("click", function (e) {
                        inp.value = this.innerText;
                        closeAllLists();
                    });

                    htmlItemContainer.appendChild(htmlItem);
                }
            }

            function highlightSubstring(text, value) {
                // Create a case-insensitive regular expression to match the value
                const regex = new RegExp(value, 'gi'); // 'g' for global, 'i' for case-insensitive

                // Replace occurrences of the value with the value wrapped in <b> tags
                const result = text.replace(regex, match => `<b>${match}</b>`);

                return result;
            }

            function addActive(x) {
                if (!x) return false;
                removeActive(x);
                if (currentFocus >= x.length) currentFocus = 0;
                if (currentFocus < 0) currentFocus = (x.length - 1);
                x[currentFocus].classList.add("autocomplete-active");
            }

            function removeActive(x) {
                for (let i = 0; i < x.length; i++) {
                    x[i].classList.remove("autocomplete-active");
                }
            }

            function closeAllLists(elmnt) {
                let x = document.getElementsByClassName("autocomplete-items");
                for (let i = 0; i < x.length; i++) {
                    if (elmnt !== x[i] && elmnt !== inp) {
                        x[i].parentNode.removeChild(x[i]);
                    }
                }
            }

            // Fecha a lista de sugestões quando o usuário clica fora do campo de entrada
            document.addEventListener("click", function (e) {
                closeAllLists(e.target);
            });
        }

        // Inicializa o autocomplete no campo de entrada
        autocomplete(element, items);
    }

    return {
        toggleValidation,
        cleanValidations,
        createToggleButtonState,
        maskInput,
        removeClasses,
        addClasses,
        replaceClasses,
        openRegulamento,
        makeAutocomplete
    }
})();

// Always executed functions
(function () {
    var regulamento = document.querySelector('.open-regulamento-menu');

    regulamento.addEventListener('click', function (event) {
        event.preventDefault();
        qrvLib.openRegulamento(false, null);
    });
})();