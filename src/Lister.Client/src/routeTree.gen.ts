/* prettier-ignore-start */

/* eslint-disable */

// @ts-nocheck

// noinspection JSUnusedGlobalSymbols

// This file is auto-generated by TanStack Router

// Import Routes

import { Route as rootRoute } from './routes/__root'
import { Route as SignInImport } from './routes/sign-in'
import { Route as AuthImport } from './routes/_auth'
import { Route as AuthIndexImport } from './routes/_auth.index'
import { Route as AuthCreateImport } from './routes/_auth.create'
import { Route as AuthListIdImport } from './routes/_auth.$listId'
import { Route as AuthListIdIndexImport } from './routes/_auth.$listId.index'
import { Route as AuthListIdCreateImport } from './routes/_auth.$listId.create'
import { Route as AuthListIdItemIdImport } from './routes/_auth.$listId.$itemId'

// Create/Update Routes

const SignInRoute = SignInImport.update({
  path: '/sign-in',
  getParentRoute: () => rootRoute,
} as any)

const AuthRoute = AuthImport.update({
  id: '/_auth',
  getParentRoute: () => rootRoute,
} as any)

const AuthIndexRoute = AuthIndexImport.update({
  path: '/',
  getParentRoute: () => AuthRoute,
} as any)

const AuthCreateRoute = AuthCreateImport.update({
  path: '/create',
  getParentRoute: () => AuthRoute,
} as any)

const AuthListIdRoute = AuthListIdImport.update({
  path: '/$listId',
  getParentRoute: () => AuthRoute,
} as any)

const AuthListIdIndexRoute = AuthListIdIndexImport.update({
  path: '/',
  getParentRoute: () => AuthListIdRoute,
} as any)

const AuthListIdCreateRoute = AuthListIdCreateImport.update({
  path: '/create',
  getParentRoute: () => AuthListIdRoute,
} as any)

const AuthListIdItemIdRoute = AuthListIdItemIdImport.update({
  path: '/$itemId',
  getParentRoute: () => AuthListIdRoute,
} as any)

// Populate the FileRoutesByPath interface

declare module '@tanstack/react-router' {
  interface FileRoutesByPath {
    '/_auth': {
      id: '/_auth'
      path: ''
      fullPath: ''
      preLoaderRoute: typeof AuthImport
      parentRoute: typeof rootRoute
    }
    '/sign-in': {
      id: '/sign-in'
      path: '/sign-in'
      fullPath: '/sign-in'
      preLoaderRoute: typeof SignInImport
      parentRoute: typeof rootRoute
    }
    '/_auth/$listId': {
      id: '/_auth/$listId'
      path: '/$listId'
      fullPath: '/$listId'
      preLoaderRoute: typeof AuthListIdImport
      parentRoute: typeof AuthImport
    }
    '/_auth/create': {
      id: '/_auth/create'
      path: '/create'
      fullPath: '/create'
      preLoaderRoute: typeof AuthCreateImport
      parentRoute: typeof AuthImport
    }
    '/_auth/': {
      id: '/_auth/'
      path: '/'
      fullPath: '/'
      preLoaderRoute: typeof AuthIndexImport
      parentRoute: typeof AuthImport
    }
    '/_auth/$listId/$itemId': {
      id: '/_auth/$listId/$itemId'
      path: '/$itemId'
      fullPath: '/$listId/$itemId'
      preLoaderRoute: typeof AuthListIdItemIdImport
      parentRoute: typeof AuthListIdImport
    }
    '/_auth/$listId/create': {
      id: '/_auth/$listId/create'
      path: '/create'
      fullPath: '/$listId/create'
      preLoaderRoute: typeof AuthListIdCreateImport
      parentRoute: typeof AuthListIdImport
    }
    '/_auth/$listId/': {
      id: '/_auth/$listId/'
      path: '/'
      fullPath: '/$listId/'
      preLoaderRoute: typeof AuthListIdIndexImport
      parentRoute: typeof AuthListIdImport
    }
  }
}

// Create and export the route tree

interface AuthListIdRouteChildren {
  AuthListIdItemIdRoute: typeof AuthListIdItemIdRoute
  AuthListIdCreateRoute: typeof AuthListIdCreateRoute
  AuthListIdIndexRoute: typeof AuthListIdIndexRoute
}

const AuthListIdRouteChildren: AuthListIdRouteChildren = {
  AuthListIdItemIdRoute: AuthListIdItemIdRoute,
  AuthListIdCreateRoute: AuthListIdCreateRoute,
  AuthListIdIndexRoute: AuthListIdIndexRoute,
}

const AuthListIdRouteWithChildren = AuthListIdRoute._addFileChildren(
  AuthListIdRouteChildren,
)

interface AuthRouteChildren {
  AuthListIdRoute: typeof AuthListIdRouteWithChildren
  AuthCreateRoute: typeof AuthCreateRoute
  AuthIndexRoute: typeof AuthIndexRoute
}

const AuthRouteChildren: AuthRouteChildren = {
  AuthListIdRoute: AuthListIdRouteWithChildren,
  AuthCreateRoute: AuthCreateRoute,
  AuthIndexRoute: AuthIndexRoute,
}

const AuthRouteWithChildren = AuthRoute._addFileChildren(AuthRouteChildren)

export interface FileRoutesByFullPath {
  '': typeof AuthRouteWithChildren
  '/sign-in': typeof SignInRoute
  '/$listId': typeof AuthListIdRouteWithChildren
  '/create': typeof AuthCreateRoute
  '/': typeof AuthIndexRoute
  '/$listId/$itemId': typeof AuthListIdItemIdRoute
  '/$listId/create': typeof AuthListIdCreateRoute
  '/$listId/': typeof AuthListIdIndexRoute
}

export interface FileRoutesByTo {
  '/sign-in': typeof SignInRoute
  '/create': typeof AuthCreateRoute
  '/': typeof AuthIndexRoute
  '/$listId/$itemId': typeof AuthListIdItemIdRoute
  '/$listId/create': typeof AuthListIdCreateRoute
  '/$listId': typeof AuthListIdIndexRoute
}

export interface FileRoutesById {
  __root__: typeof rootRoute
  '/_auth': typeof AuthRouteWithChildren
  '/sign-in': typeof SignInRoute
  '/_auth/$listId': typeof AuthListIdRouteWithChildren
  '/_auth/create': typeof AuthCreateRoute
  '/_auth/': typeof AuthIndexRoute
  '/_auth/$listId/$itemId': typeof AuthListIdItemIdRoute
  '/_auth/$listId/create': typeof AuthListIdCreateRoute
  '/_auth/$listId/': typeof AuthListIdIndexRoute
}

export interface FileRouteTypes {
  fileRoutesByFullPath: FileRoutesByFullPath
  fullPaths:
    | ''
    | '/sign-in'
    | '/$listId'
    | '/create'
    | '/'
    | '/$listId/$itemId'
    | '/$listId/create'
    | '/$listId/'
  fileRoutesByTo: FileRoutesByTo
  to:
    | '/sign-in'
    | '/create'
    | '/'
    | '/$listId/$itemId'
    | '/$listId/create'
    | '/$listId'
  id:
    | '__root__'
    | '/_auth'
    | '/sign-in'
    | '/_auth/$listId'
    | '/_auth/create'
    | '/_auth/'
    | '/_auth/$listId/$itemId'
    | '/_auth/$listId/create'
    | '/_auth/$listId/'
  fileRoutesById: FileRoutesById
}

export interface RootRouteChildren {
  AuthRoute: typeof AuthRouteWithChildren
  SignInRoute: typeof SignInRoute
}

const rootRouteChildren: RootRouteChildren = {
  AuthRoute: AuthRouteWithChildren,
  SignInRoute: SignInRoute,
}

export const routeTree = rootRoute
  ._addFileChildren(rootRouteChildren)
  ._addFileTypes<FileRouteTypes>()

/* prettier-ignore-end */

/* ROUTE_MANIFEST_START
{
  "routes": {
    "__root__": {
      "filePath": "__root.tsx",
      "children": [
        "/_auth",
        "/sign-in"
      ]
    },
    "/_auth": {
      "filePath": "_auth.tsx",
      "children": [
        "/_auth/$listId",
        "/_auth/create",
        "/_auth/"
      ]
    },
    "/sign-in": {
      "filePath": "sign-in.tsx"
    },
    "/_auth/$listId": {
      "filePath": "_auth.$listId.tsx",
      "parent": "/_auth",
      "children": [
        "/_auth/$listId/$itemId",
        "/_auth/$listId/create",
        "/_auth/$listId/"
      ]
    },
    "/_auth/create": {
      "filePath": "_auth.create.tsx",
      "parent": "/_auth"
    },
    "/_auth/": {
      "filePath": "_auth.index.tsx",
      "parent": "/_auth"
    },
    "/_auth/$listId/$itemId": {
      "filePath": "_auth.$listId.$itemId.tsx",
      "parent": "/_auth/$listId"
    },
    "/_auth/$listId/create": {
      "filePath": "_auth.$listId.create.tsx",
      "parent": "/_auth/$listId"
    },
    "/_auth/$listId/": {
      "filePath": "_auth.$listId.index.tsx",
      "parent": "/_auth/$listId"
    }
  }
}
ROUTE_MANIFEST_END */
