import { describe, it, expect } from 'vitest';
import {
  validationRules,
  combineValidators,
  validateForm,
  hasFormErrors,
} from '../utils/validation';

describe('Validation Utils', () => {
  describe('required', () => {
    it('should return error for empty string', () => {
      const validator = validationRules.required('Name');
      expect(validator('')).toBe('Name is required');
    });

    it('should return error for undefined', () => {
      const validator = validationRules.required('Name');
      expect(validator(undefined)).toBe('Name is required');
    });

    it('should return undefined for valid value', () => {
      const validator = validationRules.required('Name');
      expect(validator('John')).toBeUndefined();
    });
  });

  describe('minLength', () => {
    it('should return error for short string', () => {
      const validator = validationRules.minLength(5, 'Password');
      expect(validator('abc')).toBe('Password must be at least 5 characters');
    });

    it('should return undefined for valid length', () => {
      const validator = validationRules.minLength(5, 'Password');
      expect(validator('abcdef')).toBeUndefined();
    });
  });

  describe('maxLength', () => {
    it('should return error for long string', () => {
      const validator = validationRules.maxLength(10, 'Username');
      expect(validator('verylongusername')).toBe('Username must not exceed 10 characters');
    });

    it('should return undefined for valid length', () => {
      const validator = validationRules.maxLength(10, 'Username');
      expect(validator('short')).toBeUndefined();
    });
  });

  describe('min', () => {
    it('should return error for value below minimum', () => {
      const validator = validationRules.min(18, 'Age');
      expect(validator(15)).toBe('Age must be at least 18');
    });

    it('should return undefined for valid value', () => {
      const validator = validationRules.min(18, 'Age');
      expect(validator(20)).toBeUndefined();
    });
  });

  describe('max', () => {
    it('should return error for value above maximum', () => {
      const validator = validationRules.max(100, 'Age');
      expect(validator(120)).toBe('Age must not exceed 100');
    });

    it('should return undefined for valid value', () => {
      const validator = validationRules.max(100, 'Age');
      expect(validator(50)).toBeUndefined();
    });
  });

  describe('email', () => {
    it('should return error for invalid email', () => {
      const validator = validationRules.email();
      expect(validator('notanemail')).toBe('Invalid email address');
    });

    it('should return undefined for valid email', () => {
      const validator = validationRules.email();
      expect(validator('test@example.com')).toBeUndefined();
    });
  });

  describe('phoneNumber', () => {
    it('should return error for invalid phone', () => {
      const validator = validationRules.phoneNumber();
      expect(validator('123')).toBe('Phone number must be 10 digits');
    });

    it('should return undefined for valid phone', () => {
      const validator = validationRules.phoneNumber();
      expect(validator('1234567890')).toBeUndefined();
    });
  });

  describe('bloodPressure', () => {
    it('should return error for invalid format', () => {
      const validator = validationRules.bloodPressure();
      expect(validator('120')).toBe('Blood pressure must be in format 120/80');
    });

    it('should return error for out-of-range systolic', () => {
      const validator = validationRules.bloodPressure();
      expect(validator('300/80')).toBe('Systolic pressure must be between 60 and 250');
    });

    it('should return error for out-of-range diastolic', () => {
      const validator = validationRules.bloodPressure();
      expect(validator('120/200')).toBe('Diastolic pressure must be between 40 and 150');
    });

    it('should return undefined for valid blood pressure', () => {
      const validator = validationRules.bloodPressure();
      expect(validator('120/80')).toBeUndefined();
    });
  });

  describe('temperature', () => {
    it('should return error for low temperature', () => {
      const validator = validationRules.temperature();
      expect(validator(30)).toBe('Temperature must be between 35°C and 43°C');
    });

    it('should return error for high temperature', () => {
      const validator = validationRules.temperature();
      expect(validator(45)).toBe('Temperature must be between 35°C and 43°C');
    });

    it('should return undefined for valid temperature', () => {
      const validator = validationRules.temperature();
      expect(validator(37.2)).toBeUndefined();
    });
  });

  describe('pulseRate', () => {
    it('should return error for low pulse rate', () => {
      const validator = validationRules.pulseRate();
      expect(validator(20)).toBe('Pulse rate must be between 30 and 200 bpm');
    });

    it('should return undefined for valid pulse rate', () => {
      const validator = validationRules.pulseRate();
      expect(validator(72)).toBeUndefined();
    });
  });

  describe('oxygenSaturation', () => {
    it('should return error for low oxygen saturation', () => {
      const validator = validationRules.oxygenSaturation();
      expect(validator(50)).toBe('Oxygen saturation must be between 70% and 100%');
    });

    it('should return undefined for valid oxygen saturation', () => {
      const validator = validationRules.oxygenSaturation();
      expect(validator(98)).toBeUndefined();
    });
  });

  describe('combineValidators', () => {
    it('should return first error encountered', () => {
      const validator = combineValidators(
        validationRules.required('Name'),
        validationRules.minLength(3, 'Name')
      );
      expect(validator('')).toBe('Name is required');
    });

    it('should return undefined if all validators pass', () => {
      const validator = combineValidators(
        validationRules.required('Name'),
        validationRules.minLength(3, 'Name')
      );
      expect(validator('John')).toBeUndefined();
    });
  });

  describe('validateForm', () => {
    it('should validate all fields', () => {
      const values = {
        name: '',
        email: 'invalid',
        age: 15,
      };

      const rules = {
        name: validationRules.required('Name'),
        email: validationRules.email(),
        age: validationRules.min(18, 'Age'),
      };

      const errors = validateForm(values, rules);

      expect(errors.name).toBe('Name is required');
      expect(errors.email).toBe('Invalid email address');
      expect(errors.age).toBe('Age must be at least 18');
    });

    it('should return empty errors for valid form', () => {
      const values = {
        name: 'John',
        email: 'john@example.com',
        age: 25,
      };

      const rules = {
        name: validationRules.required('Name'),
        email: validationRules.email(),
        age: validationRules.min(18, 'Age'),
      };

      const errors = validateForm(values, rules);

      expect(errors.name).toBeUndefined();
      expect(errors.email).toBeUndefined();
      expect(errors.age).toBeUndefined();
    });
  });

  describe('hasFormErrors', () => {
    it('should return true if errors exist', () => {
      const errors = {
        name: 'Name is required',
        email: undefined,
      };
      expect(hasFormErrors(errors)).toBe(true);
    });

    it('should return false if no errors', () => {
      const errors = {
        name: undefined,
        email: undefined,
      };
      expect(hasFormErrors(errors)).toBe(false);
    });
  });
});
