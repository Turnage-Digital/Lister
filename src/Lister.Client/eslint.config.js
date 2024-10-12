import shopifyEslintPlugin from '@shopify/eslint-plugin';

export default [
  ...shopifyEslintPlugin.configs.esnext,
  ...shopifyEslintPlugin.configs.react,
];
