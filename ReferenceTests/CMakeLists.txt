cmake_minimum_required( VERSION 3.16 )

set_property( GLOBAL PROPERTY USE_FOLDERS ON )
enable_testing()

project( "ReferenceTests"
         VERSION 1.0.0
         LANGUAGES CXX
)

list( APPEND CMAKE_MODULE_PATH "${CMAKE_CURRENT_SOURCE_DIR}/src/cmake" )

include( tafc2-helpers )

tafc2_config_output_paths()

add_subdirectory( src/catch2 )
add_subdirectory( src/tests )
