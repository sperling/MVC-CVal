/*jslint white: true, browser: true, onevar: true, undef: true, nomen: true, eqeqeq: true, plusplus: true, bitwise: true, regexp: true, newcap: true, immed: true, strict: false */
/*global document: false, jQuery: false */
(function ($) {

    "use strict";

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
    function applyParams(params) {
        var commonParams = ['conditionproperty', 'validateifnot'];
        params = params || [];

        return commonParams.concat(params);
    }

    var _adaptorCache = [], // save of adaptors in hash, avoiding to make O(N) look up all the time.
        _validateRule = "cvalidate"; // our validation function name.

    // get the original adaptor function defined in jquery.validate.unobtrusive.js 
    function getAdaptor(name, lookupName) {
        var adaptor = _adaptorCache[lookupName], i;

        if (adaptor !== undefined) {
            return adaptor;
        }

        for (i = 0; i < $.validator.unobtrusive.adapters.length; i++) {
            if ($.validator.unobtrusive.adapters[i].name === name) {
                adaptor = $.validator.unobtrusive.adapters[i].adapt;

                _adaptorCache[lookupName] = adaptor;

                return adaptor;
            }
        }

        throw new Error("Can't find adapt function for validation rule " + name);
    }

    // hook up our validation function and setup the original validation through the correct adaptor.
    function setup(ruleName, options) {
        var adaptor = getAdaptor(ruleName, ruleName + 'AdaptorCache'),
                      realOptions =
                      {
                          element: options.element,
                          form: options.form,
                          message: options.message,
                          params: options.params,
                          rules: {},
                          messages: {}
                      },
                      prefix = getModelPrefix(options.element.name),
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

        if (options.rules[_validateRule] !== undefined) {
            //options.rules[_validateRule][2] = $.extend(realOptions.rules, options.rules[_validateRule][2]);
            $.extend(options.rules[_validateRule][2], realOptions.rules);
        }
        else {
            options.rules[_validateRule] = [conditionElement, ifnot, realOptions.rules];
        }

        if (options.message) {
            /*options.messages[_validateRule] = options.message;
            // remote function are using 'remote' for error message on the first time
            // when server are not returning error message when validation fails.
            // so copy over error message to 'remote'.
            if (ruleName === 'remote') {
            options.messages[ruleName] = options.message;
            }*/
            options.messages[ruleName] = options.message;
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
        var condition = !params[1],
            method,
            rule,
            ret,
            rules = params[2];

        // TODO:    make sure it works with radiobutton, select.
        if ($(params[0]).is(':checked') === condition) {
            for (method in rules) {
                if (rules.hasOwnProperty(method)) {
                    rule = { method: method, parameters: rules[method] };
                    ret = $.validator.methods[rule.method].call(this, value, element, rule.parameters);

                    if (!ret || typeof ret === 'string') {
                        // 
                        this.settings.messages[element.name][_validateRule] = this.settings.messages[element.name][method];
                        return ret;
                    }
                }
            }

            return true;
        }
        else {
            return true;
        }
    });

    $.validator.unobtrusive.adapters.add('cvrequired', applyParams(), function (options) {
        setup('required', options);
    });

    $.validator.unobtrusive.adapters.add('cvremote', applyParams(["url", "type", "additionalfields"]), function (options) {
        setup('remote', options);
    });

    $.validator.unobtrusive.adapters.add('cvlength', applyParams(['min', 'max']), function (options) {
        setup('length', options);
    });

    $.validator.unobtrusive.adapters.add('cvrange', applyParams(['min', 'max']), function (options) {
        setup('range', options);
    });

    $.validator.unobtrusive.adapters.add('cvnumber', applyParams(), function (options) {
        setup('number', options);
    });

    $.validator.unobtrusive.adapters.add('cvregex', applyParams(['pattern']), function (options) {
        setup('regex', options);
    });

    // this function depends on logic in jquery.validate.unobtrusive.js
    // orginal function is borken in the script, so this one works around
    // the problem.
    $.validator.unobtrusive.adapters.add('cvequalto', applyParams(['other']), function (options) {
        var realOptions = setup('equalto', options),
            prefix = getModelPrefix(options.element.name),
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