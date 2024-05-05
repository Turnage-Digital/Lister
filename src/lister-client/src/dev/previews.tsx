import React from "react";
import { ComponentPreview, Previews } from "@react-buddy/ide-toolbox";
import { Add, Delete, Edit } from "@mui/icons-material";

import { Loading, StatusChip, Titlebar } from "../components";

import { PaletteTree } from "./palette";

const ComponentPreviews = () => {
  const actions = [
    {
      title: "Add",
      icon: <Add />,
      onClick: () => {},
    },
    {
      title: "Edit",
      icon: <Edit />,
      onClick: () => {},
    },
    {
      title: "Delete",
      icon: <Delete />,
      onClick: () => {},
    },
  ];
  return (
    <Previews palette={<PaletteTree />}>
      <ComponentPreview path="/Loading">
        <Loading />
      </ComponentPreview>
      <ComponentPreview path="/StatusChip">
        <StatusChip />
      </ComponentPreview>
      <ComponentPreview path="/Titlebar">
        <Titlebar title="Lister" actions={actions} />
      </ComponentPreview>
    </Previews>
  );
};

export default ComponentPreviews;
