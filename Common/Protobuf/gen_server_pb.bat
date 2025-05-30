cd gentools

protoc-go --go_out=../../../project-s-server/src/server_core/src/commonmsg/protocol/ -I ../proto/ ../proto/*.proto
pause