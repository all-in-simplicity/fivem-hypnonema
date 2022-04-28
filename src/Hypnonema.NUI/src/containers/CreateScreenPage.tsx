import { FC } from "react";

import { Container, Typography } from "@mui/material";
import { ScreenForm } from "../components/ScreenForm";
import Screen from "../types/screen";
import { useNuiRequest } from "fivem-nui-react-lib";

export const CreateScreenPage: FC = () => {
  const { send } = useNuiRequest();

  const onSubmit = (data: Screen) => {
    send("createScreen", { payload: JSON.stringify(data) }).then(() => {});
  };

  let screen: Screen = {
    id: 0,
    targetSettings: {
      renderTargetName: "",
      modelName: "",
    },
    positionalSettings: {
      positionZ: 0,
      positionY: 0,
      positionX: 0,
      rotationZ: 0,
      rotationY: 0,
      rotationX: 0,
      scaleX: 0,
      scaleZ: 0,
      scaleY: 0,
    },
    browserSettings: {
      globalVolume: 100,
      soundAttenuation: 5,
      soundMinDistance: 30,
      soundMaxDistance: 200,
      is3DAudioEnabled: false,
    },
    is3DRendered: false,
    alwaysOn: false,
    name: "",
  };

  return (
    <Container>
      <Typography sx={{ marginTop: "50px", marginBottom: "10px" }} variant="h5">
        Create Screen
      </Typography>
      <ScreenForm onSubmit={(data: Screen) => onSubmit(data)} screen={screen} />
    </Container>
  );
};
