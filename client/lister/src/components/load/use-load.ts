import { useContext } from "react";

import LoadContext from "./load-context";

const useLoad = () => useContext(LoadContext);

export default useLoad;
