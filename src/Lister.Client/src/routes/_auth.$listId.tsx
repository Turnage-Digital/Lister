import { createFileRoute } from "@tanstack/react-router";

import { listDefinitionQueryOptions } from "../queryOptions";

export const Route = createFileRoute("/_auth/$listId")({
  loader: (options) =>
    options.context.queryClient.ensureQueryData(
      listDefinitionQueryOptions(options.params.listId)
    ),
});
