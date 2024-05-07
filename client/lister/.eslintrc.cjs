module.exports = {
  root: true,
  env: { browser: true, es2020: true },
  extends: [
    'eslint:recommended',
    'plugin:@typescript-eslint/recommended',
    'plugin:react-hooks/recommended',
    'plugin:@shopify/typescript',
    'plugin:@shopify/react',
    'plugin:@shopify/prettier'
  ],
  ignorePatterns: ['dist', '.eslintrc.cjs', '*.js'],
  parser: '@typescript-eslint/parser',
  plugins: ['react-refresh'],
  rules: {
    'react-refresh/only-export-components': [
      'warn',
      { allowConstantExport: true },
    ],
    'id-length': 'off',
    'no-process-env': 'off',
    '@shopify/jsx-no-hardcoded-content': 'off',
    '@typescript-eslint/naming-convention': 'off'
  },
}