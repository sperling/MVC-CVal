(function ($) {

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
                if (jQuery.inArray(params[i], ['minlength', 'maxlength', 'min', 'max']) > -1) {
                    params[i + 1] = Number(params[i + 1]);
                }
                else if (jQuery.inArray(params[i], ['rangelength', 'range']) > -1) {
                    params[i + 1] = Number(params[i + 1]);
                    params[i + 2] = Number(params[i + 2]);
                }
                options.rules[ruleName].push(params[i]);
            }
        }

        if (options.message) {
            options.messages[ruleName] = options.message;
        }

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

    jQuery.validator.addMethod("cvrequired", function (value, element, params) {
        var that = this;
        return Validate(params[0], params[1], function () {
            return jQuery.validator.methods['required'].call(that, value, element, true);
        });
    });

    jQuery.validator.unobtrusive.adapters.add('cvrequired', ApplyParams(), function (options) {
        // jQuery Validate equates "required" with "mandatory" for checkbox elements
        if (options.element.tagName.toUpperCase() !== "INPUT" || options.element.type.toUpperCase() !== "CHECKBOX") {
            Init(options, 'cvrequired');
        }
    });

    jQuery.validator.addMethod("cvminmax", function (value, element, params) {
        var that = this;
        return Validate(params[0], params[1], function () {
            if (params.length > 4) {
                return jQuery.validator.methods[params[2]].call(that, value, element, params.slice(3));
            }
            else {
                return jQuery.validator.methods[params[2]].call(that, value, element, params[3]);
            }
        });
    });

    jQuery.validator.unobtrusive.adapters.add('cvlength', ApplyParams(['min', 'max']), function (options) {
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
    });

    jQuery.validator.unobtrusive.adapters.add('cvrange', ApplyParams(['min', 'max']), function (options) {
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

    jQuery.validator.addMethod("cvnumber", function (value, element, params) {
        var that = this;
        return Validate(params[0], params[1], function () {
            return jQuery.validator.methods['number'].call(that, value, element, true);
        });
    });

    jQuery.validator.unobtrusive.adapters.add('cvnumber', ApplyParams(), function (options) {
        Init(options, 'cvnumber');
    });

    jQuery.validator.addMethod("cvregex", function (value, element, params) {
        var that = this;
        return Validate(params[0], params[1], function () {
            return jQuery.validator.methods['regex'].call(that, value, element, params[2]);
        });
    });

    jQuery.validator.unobtrusive.adapters.add('cvregex', ApplyParams(['pattern']), function (options) {
        Init(options, 'cvregex', [options.params.pattern]);
    });

    jQuery.validator.addMethod("cvequalto", function (value, element, params) {
        var that = this;
        return Validate(params[0], params[1], function () {
            return jQuery.validator.methods['equalTo'].call(that, value, element, params[2]);
        });
    });

    jQuery.validator.unobtrusive.adapters.add('cvequalto', ApplyParams(['other']), function (options) {
        // from adapters.add("equalto")
        var prefix = getModelPrefix(options.element.name),
            other = options.params.other,
            fullOtherName = appendModelPrefix(other, prefix),
            element = $(options.form).find(":input[name='" + fullOtherName + "']")[0]; // OBS!!!   need the ' for exact match, was missing in mvc.

        Init(options, 'cvequalto', [element]);
    });

} (jQuery));