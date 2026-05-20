// Validation rules
export type ValidationRule<T = any> = (value: T) => string | undefined;

// Common validation rules
export const validationRules = {
  required: (fieldName: string = 'This field'): ValidationRule => {
    return (value: any) => {
      if (value === undefined || value === null || value === '') {
        return `${fieldName} is required`;
      }
      if (typeof value === 'string' && value.trim() === '') {
        return `${fieldName} is required`;
      }
      return undefined;
    };
  },

  minLength: (min: number, fieldName: string = 'This field'): ValidationRule<string> => {
    return (value: string) => {
      if (value && value.length < min) {
        return `${fieldName} must be at least ${min} characters`;
      }
      return undefined;
    };
  },

  maxLength: (max: number, fieldName: string = 'This field'): ValidationRule<string> => {
    return (value: string) => {
      if (value && value.length > max) {
        return `${fieldName} must not exceed ${max} characters`;
      }
      return undefined;
    };
  },

  min: (minValue: number, fieldName: string = 'Value'): ValidationRule<number> => {
    return (value: number) => {
      if (value !== undefined && value !== null && value < minValue) {
        return `${fieldName} must be at least ${minValue}`;
      }
      return undefined;
    };
  },

  max: (maxValue: number, fieldName: string = 'Value'): ValidationRule<number> => {
    return (value: number) => {
      if (value !== undefined && value !== null && value > maxValue) {
        return `${fieldName} must not exceed ${maxValue}`;
      }
      return undefined;
    };
  },

  pattern: (regex: RegExp, message: string): ValidationRule<string> => {
    return (value: string) => {
      if (value && !regex.test(value)) {
        return message;
      }
      return undefined;
    };
  },

  email: (): ValidationRule<string> => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return validationRules.pattern(emailRegex, 'Invalid email address');
  },

  phoneNumber: (): ValidationRule<string> => {
    const phoneRegex = /^\d{10}$/;
    return validationRules.pattern(phoneRegex, 'Phone number must be 10 digits');
  },

  bloodPressure: (): ValidationRule<string> => {
    const bpRegex = /^\d{2,3}\/\d{2,3}$/;
    return (value: string) => {
      if (!value) return undefined;
      if (!bpRegex.test(value)) {
        return 'Blood pressure must be in format 120/80';
      }
      const [systolic, diastolic] = value.split('/').map(Number);
      if (systolic < 60 || systolic > 250) {
        return 'Systolic pressure must be between 60 and 250';
      }
      if (diastolic < 40 || diastolic > 150) {
        return 'Diastolic pressure must be between 40 and 150';
      }
      return undefined;
    };
  },

  temperature: (): ValidationRule<number> => {
    return (value: number) => {
      if (value === undefined || value === null) return undefined;
      if (value < 35 || value > 43) {
        return 'Temperature must be between 35°C and 43°C';
      }
      return undefined;
    };
  },

  pulseRate: (): ValidationRule<number> => {
    return (value: number) => {
      if (value === undefined || value === null) return undefined;
      if (value < 30 || value > 200) {
        return 'Pulse rate must be between 30 and 200 bpm';
      }
      return undefined;
    };
  },

  oxygenSaturation: (): ValidationRule<number> => {
    return (value: number) => {
      if (value === undefined || value === null) return undefined;
      if (value < 70 || value > 100) {
        return 'Oxygen saturation must be between 70% and 100%';
      }
      return undefined;
    };
  },
};

// Combine multiple validation rules
export const combineValidators = <T = any>(
  ...validators: ValidationRule<T>[]
): ValidationRule<T> => {
  return (value: T) => {
    for (const validator of validators) {
      const error = validator(value);
      if (error) {
        return error;
      }
    }
    return undefined;
  };
};

// Form validation helper
export interface FieldErrors {
  [key: string]: string | undefined;
}

export const validateForm = <T extends Record<string, any>>(
  values: T,
  rules: { [K in keyof T]?: ValidationRule<T[K]> }
): FieldErrors => {
  const errors: FieldErrors = {};

  for (const field in rules) {
    const rule = rules[field];
    if (rule) {
      const error = rule(values[field]);
      if (error) {
        errors[field] = error;
      }
    }
  }

  return errors;
};

// Check if form has errors
export const hasFormErrors = (errors: FieldErrors): boolean => {
  return Object.values(errors).some((error) => error !== undefined);
};
