import shopifyEslintPlugin from "@shopify/eslint-plugin";

// eslint-disable-next-line import/no-anonymous-default-export
export default [
  ...shopifyEslintPlugin.configs.typescript,
  ...shopifyEslintPlugin.configs["typescript-type-checking"],
  {
    languageOptions: {
      parserOptions: {
        project: "tsconfig.json",
      },
    },
  },
  ...shopifyEslintPlugin.configs.react,
  ...shopifyEslintPlugin.configs.prettier,
  {
    rules: {
      "@shopify/jsx-no-hardcoded-content": "off",
      "@typescript-eslint/naming-convention": "off",
      "id-length": "off",
      "no-process-env": "off",
      "no-implicit-coercion": "off",
      "no-template-curly-in-string": "off",
    },
  },
];
