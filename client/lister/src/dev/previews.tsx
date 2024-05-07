import React from "react";
import { ComponentPreview, Previews } from "@react-buddy/ide-toolbox";

import { Loading } from "../components";

import { PaletteTree } from "./palette";

const ComponentPreviews = () => {
  return (
    <Previews palette={<PaletteTree />}>
      <ComponentPreview path="/Loading">
        <Loading />
      </ComponentPreview>
    </Previews>
  );
};

export default ComponentPreviews;
