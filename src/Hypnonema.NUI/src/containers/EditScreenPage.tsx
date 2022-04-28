import {Container, Typography} from "@mui/material";
import {FC} from "react";
import {useAppSelector} from "../hooks/store";
import {ScreenForm} from "../components/ScreenForm";
import {useParams} from "react-router";
import Screen from "../types/screen";
import {useNuiRequest} from "fivem-nui-react-lib";

export const EditScreenPage: FC = () => {
    const {screenId} = useParams();

    const {send} = useNuiRequest();

    const screen = useAppSelector((state) => state.screens.screens?.find(s => s.id === parseInt(screenId!)))

    const onSubmit = (data: Screen) => {
        send('editScreen', {payload: JSON.stringify(data)}).then(() => {});
    }

    return (
        <Container>
            <Typography sx={{marginTop: '50px', marginBottom: '10px'}} variant="h5">Edit Screen</Typography>
            <ScreenForm onSubmit={(data: Screen) => onSubmit(data)} screen={screen}/>
        </Container>
    )
}
