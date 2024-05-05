import React from "react";
import { ComponentPreview, Previews } from "@react-buddy/ide-toolbox";

import { FormBlock, Loading, StatusChip } from "../components";

import { PaletteTree } from "./palette";

const ComponentPreviews = () => {
  return (
    <Previews palette={<PaletteTree />}>
      <ComponentPreview path="/Loading">
        <Loading />
      </ComponentPreview>
      <ComponentPreview path="/StatusChip">
        <StatusChip />
      </ComponentPreview>
    </Previews>
  );
};

export default ComponentPreviews;
