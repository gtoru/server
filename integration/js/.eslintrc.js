module.exports = {
    root: true,
    parser: "@typescript-eslint/parser",
    parserOptions: {
        "ecmaVersion": 6,
        "sourceType": "module",
        "ecmaFeatures": {
            "impliedStrict": true
        }
    },
    plugins: [
        "@typescript-eslint",
        "prettier",
        "jest",
        "jest-formatting"
    ],
    extends: [
        "plugin:@typescript-eslint/eslint-recommended",
        "plugin:@typescript-eslint/recommended",
        "plugin:prettier/recommended",
        "plugin:jest/recommended",
        "plugin:jest/style",
        "plugin:jest-formatting/recommended",
        "eslint:recommended"
    ],
    env: {
        "node": true,
        "es6": true,
        "jest/globals": true
    },
    "rules": {
        "prettier/prettier": "error",
        "jest/no-disabled-tests": "warn",
        "jest/no-focused-tests": "error",
        "jest/no-identical-title": "error",
        "jest/prefer-to-have-length": "warn",
        "jest/valid-expect": "error"
    }
};
