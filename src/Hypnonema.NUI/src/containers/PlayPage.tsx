import { FC } from "react";
import { Container, Typography } from "@mui/material";

import { PlayForm } from "../components/PlayForm";
import { useAppSelector } from "../hooks/store";
import { useNuiRequest } from "fivem-nui-react-lib";

export const PlayPage: FC = () => {
  const screens = useAppSelector((state) => state.screens.screens);

  const { send } = useNuiRequest();

  const onSubmit = (screenId: number, videoUrl: string) => {
    const screen = screens?.find((s) => s.id === screenId);

    if (screen)
      send("play", { screen: JSON.stringify(screen), videoUrl }).then(() => {});
  };

  return (
    <Container>
      <Typography sx={{ marginTop: "50px", marginBottom: "10px" }} variant="h5">
        Play
      </Typography>
      <PlayForm onSubmit={onSubmit} screens={screens || []} />
    </Container>
  );
};
