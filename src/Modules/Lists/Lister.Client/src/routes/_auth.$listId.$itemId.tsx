import {createFileRoute} from "@tanstack/react-router";

import {itemQueryOptions} from "../query-options";

export const Route = createFileRoute("/_auth/$listId/$itemId")({
    params: {
        stringify: (params: any) => {
            return {
                listId: params.listId as string,
                itemId: params.itemId.toString(),
            };
        },
    },
    loader: (options) => {
        options.context.queryClient.ensureQueryData(
            itemQueryOptions(options.params.listId, options.params.itemId),
        );
    },
});
