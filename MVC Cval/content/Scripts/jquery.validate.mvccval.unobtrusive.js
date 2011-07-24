(function ($) {

    // taken from jquery.validate.unobtrusive.js
    function splitAndTrim(value) {
        return value.replace(/^\s+|\s+$/g, "").split(/\s*,\s*/g);
    }

    // taken from jquery.validate.unobtrusive.js
    function getModelPrefix(fieldName) {
        return fieldName.substr(0, fieldName.lastIndexOf(".") + 1);
    }

    // taken from jquery.validate.unobtrusive.js
    function appendModelPrefix(value, prefix) {
        if (value.indexOf("*.") === 0) {
            value = value.replace("*.", prefix);
        }
        return value;
    }

    // the first two properties should always be included.
    // add any extra params after.
    function ApplyParams(params) {
        var commonParams = ['conditionproperty', 'validateifnot'];
        params = params || [];

        return commonParams.concat(params);
    }

    // save of adaptors in hash, avoiding to make O(N) look up all the time.
    var _adaptorCache = [];

    // get the original adaptor function defined in jquery.validate.unobtrusive.js 
    function GetAdaptor(name, lookupName) {
        var adaptor = _adaptorCache[lookupName];

        if (adaptor !== undefined) {
            return adaptor;
        }

        for (var i = 0; i < $.validator.unobtrusive.adapters.length; i++) {
            if ($.validator.unobtrusive.adapters[i].name === name) {
                adaptor = $.validator.unobtrusive.adapters[i].adapt;

                _adaptorCache[lookupName] = adaptor;

                return adaptor;
            }
        }

        throw new Error("Can't find adapt function for validation rule " + name);
    }

    // our validation function name.
    var _validateRule = "cvalidate";

    // hook up our validation function and setup the original validation through the correct adaptor.
    function Setup(ruleName, options) {
        var adaptor = GetAdaptor(ruleName, ruleName + 'AdaptorCache');
        var realOptions =
        {
            element: options.element,
            form: options.form,
            message: options.message,
            params: options.params,
            rules: {},
            messages: {}
        };
        var prefix = getModelPrefix(options.element.name),
            conditionProperty = options.params.conditionproperty,
            fullConditionName = appendModelPrefix(conditionProperty, prefix),
            conditionElement = $(options.form).find(":input[name='" + fullConditionName + "']")[0],       // OBS!!!   need the ' for exact match. look at 'cvequalto' below.
            ifnot = (/^true$/i).test(options.params.validateifnot);

        adaptor(realOptions);

        if (ruleName === 'required' && realOptions.rules[ruleName] === undefined) {
            // not added, for required...
            return null;
        }

        // normalizeRules() are not executed for original rules, so
        // need to run it for them. else range and length rules will not have parameters converted to Number.
        $.validator.normalizeRules(realOptions.rules, realOptions.element);

        options.rules[_validateRule] = [conditionElement, ifnot, realOptions.rules];
        if (options.message) {
            options.messages[_validateRule] = options.message;
            // remote function are using 'remote' for error message on the first time
            // when server are not returning error message when validation fails.
            // so copy over error message to 'remote'.
            if (ruleName === 'remote') {
                options.messages[ruleName] = options.message;
            }
        }

        // FIXME:   live here?
        $(conditionElement).change(function () {
            // run validation for element again if condition is not true anymore.
            // hide error message.
            // TODO:    make sure it works with radiobutton, select.
            if ($(this).is(':checked') === ifnot) {
                $(options.form).validate().element(options.element);
            }
        });

        return realOptions;
    }

    $.validator.addMethod(_validateRule, function (value, element, params) {
        var condition = !params[1];

        // TODO:    make sure it works with radiobutton, select.
        if ($(params[0]).is(':checked') === condition) {
            for (var method in params[2]) {
                var rule = { method: method, parameters: params[2][method] };
                var ret = $.validator.methods[rule.method].call(this, value, element, rule.parameters);

                if (!ret) {
                    return ret;
                }
            }

            return true;
        }
        else {
            return true;
        }
    });

    $.validator.unobtrusive.adapters.add('cvrequired', ApplyParams(), function (options) {
        Setup('required', options);
    });

    $.validator.unobtrusive.adapters.add('cvremote', ApplyParams(["url", "type", "additionalfields"]), function (options) {
        Setup('remote', options);
    });

    $.validator.unobtrusive.adapters.add('cvlength', ApplyParams(['min', 'max']), function (options) {
        Setup('length', options);
    });

    $.validator.unobtrusive.adapters.add('cvrange', ApplyParams(['min', 'max']), function (options) {
        Setup('range', options);
    });

    $.validator.unobtrusive.adapters.add('cvnumber', ApplyParams(), function (options) {
        Setup('number', options);
    });

    $.validator.unobtrusive.adapters.add('cvregex', ApplyParams(['pattern']), function (options) {
        Setup('regex', options);
    });

    // this function depends on logic in jquery.validate.unobtrusive.js
    // orginal function is borken in the script, so this one works around
    // the problem.
    $.validator.unobtrusive.adapters.add('cvequalto', ApplyParams(['other']), function (options) {
        var realOptions = Setup('equalto', options);
        var prefix = getModelPrefix(options.element.name),
                     other = options.params.other,
                     fullOtherName = appendModelPrefix(other, prefix);
        // mvc script is broken, $(options.form).find(":input[name=" + fullOtherName + "]")[0]
        // will find all elements if using prefix(got a '.').
        // so it can pick the wrong element(first one). change to correct one if wrong element is used.
        if (realOptions.rules.equalTo.name !== fullOtherName) {
            realOptions.rules.equalTo = $(options.form).find(":input[name='" + fullOtherName + "']")[0];
        }

    });
} (jQuery));