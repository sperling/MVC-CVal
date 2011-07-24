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

    function Init(options, ruleName, params) {
        var prefix = getModelPrefix(options.element.name),
            conditionProperty = options.params.conditionproperty,
            fullConditionName = appendModelPrefix(conditionProperty, prefix),
            conditionElement = $(options.form).find(":input[name='" + fullConditionName + "']")[0],       // OBS!!!   need the ' for exact match, was missing in mvc.
            ifnot = (/^true$/i).test(options.params.validateifnot);
        var i;

        options.rules[ruleName] = [conditionElement, ifnot];

        if (params !== undefined) {

            // add remaning parameters needed for validation function.
            for (i = 0; i < params.length; i++) {
                // we need to convert to numbers for this. taken from
                // jQuery.validation.normalizeRules(), we are not using orginal names(prefix 'cv'), so are not trigged.
                // clean number parameters
                if ($.inArray(params[i], ['minlength', 'maxlength', 'min', 'max']) > -1) {
                    params[i + 1] = Number(params[i + 1]);
                }
                else if ($.inArray(params[i], ['rangelength', 'range']) > -1) {
                    params[i + 1] = Number(params[i + 1]);
                    params[i + 2] = Number(params[i + 2]);
                }
                options.rules[ruleName].push(params[i]);
            }
        }

        if (options.message) {
            options.messages[ruleName] = options.message;

            // remote function are using 'remote' for error message on the first time
            // when server are not returning error message when validation fails.
            // so copy over error message to 'remote'.
            if (ruleName === 'cvremote') {
                options.messages[ruleName.substring(2)] = options.message;
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
    }

    function Validate(conditionProperty, validateIfNot, callback) {
        var condition = !validateIfNot;

        // TODO:    make sure it works with radiobutton, select.
        if ($(conditionProperty).is(':checked') === condition) {
            return callback();
        }
        else {
            return true;
        }
    }

    function ApplyParams(params) {
        var commonParams = ['conditionproperty', 'validateifnot'];
        params = params || [];

        return commonParams.concat(params);
    }

    // TODO:    can we reuse jQuery.validator.unobtrusive.adapters below?
    //          avoid copy&paste somehow... need to have same logic.

    /*$.validator.addMethod("cvrequired", function (value, element, params) {
    var that = this;
    return Validate(params[0], params[1], function () {
    return $.validator.methods['required'].call(that, value, element, true);
    });
    });*/

    /*$.validator.unobtrusive.adapters.add('cvrequired', ApplyParams(), function (options) {
    // jQuery Validate equates "required" with "mandatory" for checkbox elements
    if (options.element.tagName.toUpperCase() !== "INPUT" || options.element.type.toUpperCase() !== "CHECKBOX") {
    Init(options, 'cvrequired');
    }
    });*/

    function GetAdaptor(name) {
        for (var i = 0; i < $.validator.unobtrusive.adapters.length; i++) {
            if ($.validator.unobtrusive.adapters[i].name === name) {
                return $.validator.unobtrusive.adapters[i].adapt;
            }
        }

        throw new Error("Can't find adapt function for validation rule " + name);
    }

    var _validateRule = "cvalidate";

    function Setup(ruleName, options) {
        var adaptor = GetAdaptor(ruleName);
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
            conditionElement = $(options.form).find(":input[name='" + fullConditionName + "']")[0],       // OBS!!!   need the ' for exact match, was missing in mvc.
            ifnot = (/^true$/i).test(options.params.validateifnot);

        adaptor(realOptions);

        if (ruleName === 'required' && realOptions.rules[ruleName] === undefined) {
            // not added, for required...
            return;
        }

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

    $.validator.addMethod("cvminmax", function (value, element, params) {
        var that = this;
        return Validate(params[0], params[1], function () {
            if (params.length > 4) {
                return $.validator.methods[params[2]].call(that, value, element, params.slice(3));
            }
            else {
                return $.validator.methods[params[2]].call(that, value, element, params[3]);
            }
        });
    });

    /*$.validator.unobtrusive.adapters.add('cvlength', ApplyParams(['min', 'max']), function (options) {
    // from adapters.addMinMax
    // have three rules here.
    var min = options.params.min,
    max = options.params.max;

    if (min && max) {
    Init(options, 'cvminmax', ['rangelength', min, max]);
    }
    else if (min) {
    Init(options, 'cvminmax', ['minlength', min]);
    }
    else if (max) {
    Init(options, 'cvminmax', ['maxlength', max]);
    }
    });*/

    $.validator.unobtrusive.adapters.add('cvrange', ApplyParams(['min', 'max']), function (options) {
        // from adapters.addMinMax
        // have three rules here.
        var min = options.params.min,
            max = options.params.max;

        if (min && max) {
            Init(options, 'cvminmax', ['range', min, max]);
        }
        else if (min) {
            Init(options, 'cvminmax', ['min', min]);
        }
        else if (max) {
            Init(options, 'cvminmax', ['max', max]);
        }
    });

    $.validator.addMethod("cvnumber", function (value, element, params) {
        var that = this;
        return Validate(params[0], params[1], function () {
            return $.validator.methods['number'].call(that, value, element, true);
        });
    });

    $.validator.unobtrusive.adapters.add('cvnumber', ApplyParams(), function (options) {
        Init(options, 'cvnumber');
    });

    $.validator.addMethod("cvregex", function (value, element, params) {
        var that = this;
        return Validate(params[0], params[1], function () {
            return $.validator.methods['regex'].call(that, value, element, params[2]);
        });
    });

    $.validator.unobtrusive.adapters.add('cvregex', ApplyParams(['pattern']), function (options) {
        Init(options, 'cvregex', [options.params.pattern]);
    });

    $.validator.addMethod("cvequalto", function (value, element, params) {
        var that = this;
        return Validate(params[0], params[1], function () {
            return $.validator.methods['equalTo'].call(that, value, element, params[2]);
        });
    });

    $.validator.unobtrusive.adapters.add('cvequalto', ApplyParams(['other']), function (options) {
        // from adapters.add("equalto")
        var prefix = getModelPrefix(options.element.name),
            other = options.params.other,
            fullOtherName = appendModelPrefix(other, prefix),
            element = $(options.form).find(":input[name='" + fullOtherName + "']")[0]; // OBS!!!   need the ' for exact match, was missing in mvc.

        Init(options, 'cvequalto', [element]);
    });

    /*$.validator.addMethod("cvremote", function (value, element, params) {
    var that = this;
    return Validate(params[0], params[1], function () {
    return $.validator.methods['remote'].call(that, value, element, params[2]);
    });
    });

    $.validator.unobtrusive.adapters.add('cvremote', ApplyParams(["url", "type", "additionalfields"]), function (options) {
    // from adapters.add("remote")
    var value = {
    url: options.params.url,
    type: options.params.type || "GET",
    data: {}
    },
    prefix = getModelPrefix(options.element.name);

    $.each(splitAndTrim(options.params.additionalfields || options.element.name), function (i, fieldName) {
    var paramName = appendModelPrefix(fieldName, prefix);
    value.data[paramName] = function () {
    return $(options.form).find(":input[name='" + paramName + "']").val();
    };
    });

    Init(options, 'cvremote', [value]);
    });*/

} (jQuery));