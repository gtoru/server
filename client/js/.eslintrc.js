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
    ],
    extends: [
        "plugin:@typescript-eslint/eslint-recommended",
        "plugin:@typescript-eslint/recommended",
        "plugin:prettier/recommended",
        "eslint:recommended"
    ],
    env: {
        "node": true,
        "es6": true,
    },
    "rules": {
        "prettier/prettier": "error"
    }
};
