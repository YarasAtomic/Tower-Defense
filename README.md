# Esiten

## Prueba del código fuente

Para verificar el funcionamiento del código se deben seguir una serie de pasos para la configuración del editor de Unity:

1. Abrir el código fuente (*source code*) como proyecto de Unity.
2. Abrir la escena *'MainMenu'* en la carpeta *'Scenes'*.
3. Pulsar en el botón *Play*.

## Descripción

Se trata de un juego del género ***tower defense*** desarrollado en Unity para la asignatura PGV. Algunas de las características son:

- **Archivos de guardado:** donde se almacena el progreso del jugador. Se permite hasta tres guardados simultáneos.
- **Menú de inicio:** en el que seleccionar la partida que se quiere jugar o se puede crear una nueva.
- **Menú de niveles:** con un mapa en el se muestran las localizaciones que se desbloquean conforme se avanza en el juego.
- **Niveles:** de dificultad creciente y en los que se debe eliminar a todos los enemigos antes de que estos eliminen la base haciendo uso de torretas defensivas con diferentes características.

## Detalles de desarrollo

Se han usado distintos tipos de técnicas y herramientas para implementar toda la funcionalidad descrita:

### Visual

Texturas y materiales para los objetos del juego, shaders de cara a obtener un mejor aspecto visual y diferentes efectos visuales para recrear explosiones y efectos de humo y niebla.

### IU

Componentes de la interfaz y botones que permiten la interacción con los menús, el mundo y los niveles. Dentro de estos últimos se ofrecen varias cámaras: una cenital de toda la escena y cámaras de seguimiento de los enemigos y las torretas en primera persona.

### Metodología

En cuanto al código, se ha tratado de emplear técnicas para su reutilización y una mejor comprensión como herencia, reutilización de instancias, modularidad de los componentes y los niveles, entre otras.

## Ejecución

En la sección *releases* de esta página se ofrece la posibilidad de descargar la *build* más reciente del juego, así como las versiones anteriores. Los pasos para poder ejecutarlo serían:

1. **Descargar la *build*:** eligiendo el paquete según el sistema operativo en el que se va a ejecutar (Windows o Linux).
2. **Descomprimir el contenido:** en la ruta deseada.
3. **Ejecutar el juego:** en el caso de Windows se ejecuta el archivo *'Esiten.exe'*, en el caso de Linux se ejecuta el archivo *'Esiten.x86_64'*.

## Créditos

### Desarrolladores

Los participantes en este proyecto han sido:

- Guillermo Medialdea Burgos
- Pablo Quesada Rojo
- Ignacio Moya Montoro
- José Pérez Vidal
- Paulina Senado Montoya
- Lorena Gómez Gómez

### Assets externos

- [Música para el nivel](https://chanwalrus.com/sci-fi-music/)
- [Música para el menú](https://pixabay.com/sound-effects/wrong-place-129242/)
- [SFX de entrada a un nivel](https://pixabay.com/sound-effects/going-to-the-next-level-114480/)
