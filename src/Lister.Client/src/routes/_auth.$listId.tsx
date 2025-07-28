import { createFileRoute } from "@tanstack/react-router";

import { listItemDefinitionQueryOptions } from "../query-options";

export const Route = createFileRoute("/_auth/$listId")({
  loader: (options) =>
    options.context.queryClient.ensureQueryData(
      listItemDefinitionQueryOptions(options.params.listId),
    ),
});
