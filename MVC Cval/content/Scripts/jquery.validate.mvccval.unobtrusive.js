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

    var _adaptorCache = []; // save of adaptors in hash, avoiding to make O(N) look up all the time.

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
        var cvRuleName = 'cv' + ruleName,
            adaptor = getAdaptor(ruleName, cvRuleName),
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

        options.rules[cvRuleName] = [conditionElement, ifnot, realOptions.rules];

        if (options.message) {
            options.messages[cvRuleName] = options.message;
            // remote function are using 'remote' for error message on the first time
            // when server are not returning error message when validation fails.
            // so copy over error message to 'remote' for showing correct message.
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

    // do the validation if condition is true.
    function cvalidate(value, element, params) {
        var condition = !params[1],
            ret,
            rules = params[2],
            method;

        // TODO:    make sure it works with radiobutton, select.
        if ($(params[0]).is(':checked') === condition) {
            for (method in rules) {
                if (rules.hasOwnProperty(method)) {
                    ret = $.validator.methods[method].call(this, value, element, rules[method]);

                    if (!ret || typeof ret === 'string') {
                        return ret;
                    }
                }
            }
            return true;
        }
        else {
            return true;
        }
    }

    $.validator.addMethod('cvrequired', function (value, element, params) {
        return cvalidate.call(this, value, element, params);
    });

    $.validator.addMethod('cvremote', function (value, element, params) {
        return cvalidate.call(this, value, element, params);
    });

    $.validator.addMethod('cvlength', function (value, element, params) {
        return cvalidate.call(this, value, element, params);
    });

    $.validator.addMethod('cvrange', function (value, element, params) {
        return cvalidate.call(this, value, element, params);
    });

    $.validator.addMethod('cvnumber', function (value, element, params) {
        return cvalidate.call(this, value, element, params);
    });

    $.validator.addMethod('cvregex', function (value, element, params) {
        return cvalidate.call(this, value, element, params);
    });

    $.validator.addMethod('cvequalto', function (value, element, params) {
        return cvalidate.call(this, value, element, params);
    });

    // OBS!!!   keep order added.
    $.validator.unobtrusive.adapters.add('cvregex', applyParams(['pattern']), function (options) {
        setup('regex', options);
    });

    $.validator.unobtrusive.adapters.add('cvnumber', applyParams(), function (options) {
        setup('number', options);
    });

    $.validator.unobtrusive.adapters.add('cvlength', applyParams(['min', 'max']), function (options) {
        setup('length', options);
    });

    $.validator.unobtrusive.adapters.add('cvrange', applyParams(['min', 'max']), function (options) {
        setup('range', options);
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

    $.validator.unobtrusive.adapters.add('cvrequired', applyParams(), function (options) {
        setup('required', options);
    });

    $.validator.unobtrusive.adapters.add('cvremote', applyParams(["url", "type", "additionalfields"]), function (options) {
        setup('remote', options);
    });


} (jQuery));