import { useContext } from "react";

import ListDefinitionContext from "./list-definition-context";

const useListDefinition = () => useContext(ListDefinitionContext);

export default useListDefinition;
